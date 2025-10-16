using System.Threading.Channels;
using SynologyNas.Api;

namespace SynologyNas.FilesIndex
{
    public sealed class FolderIndexer
    {
        private readonly ListApiService _api;
        private readonly IndexWriterWorker _fileWriterWorker;
        private readonly IFilesProgressReporter _progress;
        private readonly int _maxParallel;

        public FolderIndexer(ListApiService api, IndexWriterWorker fileWriterWorker, IFilesProgressReporter progress,
            int maxParallel = 5)
        {
            _api = api;
            _fileWriterWorker = fileWriterWorker;
            _progress = progress;
            _maxParallel = maxParallel;
        }

        public async Task IndexAsync(string rootFolder, CancellationToken token = default)
        {
            var folderChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false
            });

            await folderChannel.Writer.WriteAsync(rootFolder, token);

            int activeProducers = 0;

            var workers = Enumerable.Range(0, _maxParallel)
                .Select(_ => Task.Run(() => FolderWorkerAsync(folderChannel,
                () => {
                    Interlocked.Increment(ref activeProducers);
                    _progress.SetActiveWorckersCount(Volatile.Read(ref activeProducers));
                },
                () => {
                    Interlocked.Decrement(ref activeProducers);
                    _progress.SetActiveWorckersCount(Volatile.Read(ref activeProducers));
                }, token)))
                .ToArray();

            _progress.Start();

            // Монитор следит, когда можно закрывать канал
            var monitorTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(500, token);
                    if (folderChannel.Reader.Count == 0 && Volatile.Read(ref activeProducers) == 0)
                    {
                        folderChannel.Writer.Complete();
                        break;
                    }
                }
            }, token);

            await Task.WhenAll(workers.Concat(new[] { monitorTask }));

            await _progress.StopAsync();
        }

        private async Task FolderWorkerAsync(Channel<string> folderChannel, Action onStart, Action onFinish,
            CancellationToken token)
        {
            await foreach (var folder in folderChannel.Reader.ReadAllAsync(token))
            {
                int offset = 0;
                try
                {
                    onStart();
                    _progress.IncrementFolder();

                    const int limit = 1000;
                    bool done;

                    do
                    {
                        var (items, isComplete) = await _api.ListFolderAsync(folder, limit, offset, token);
                        done = isComplete;
                        offset += limit;

                        foreach (var item in items)
                        {
                            bool isDir = item["isdir"]?.ToObject<bool>() ?? false;
                            string path = item["path"]?.ToString() ?? "";
                            if (isDir)
                            {
                                await folderChannel.Writer.WriteAsync(path, token);
                            }
                            else
                            {
                                long? mtime = item["additional"]?["time"]?["mtime"]?.ToObject<long>();
                                DateTime modified = mtime.HasValue
                                ? DateTimeOffset.FromUnixTimeSeconds(mtime.Value).UtcDateTime
                                : DateTime.MinValue;

                                var entry = new FileEntry
                                {
                                    Path = path,
                                    Name = item["name"]?.ToString() ?? "",
                                    Owner = item["additional"]?["owner"]?["user"]?.ToString() ?? "",
                                    Size = item["additional"]?["size"]?.ToObject<int>() ?? 0,
                                    Modified = modified,
                                    Extension = item["additional"]?["type"]?.ToString() ?? "",
                                    IsDirectory = isDir
                                };
                                await _fileWriterWorker.EnqueueAsync(entry);
                                _progress.IncrementFile();
                            }
                        }
                    } while (!done);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обходе '{folder}', offset - {offset}: {ex.Message}");
                }
                finally
                {
                    onFinish();
                }
            }
        }
    }
}