using System.Diagnostics;

namespace SynologyNas.FilesIndex
{
    public class FileIndexProgressReporter : IFilesProgressReporter
    {
        private long _filesWritten = 0;
        private long _foldersProcessed = 0;
        private int _activeWorckersCount = 0;
        private readonly Stopwatch _stopwatch = new();
        private readonly TimeSpan _interval;
        private readonly CancellationToken _externalToken;
        private CancellationTokenSource? _cts;
        private Task? _reportTask;

        public FileIndexProgressReporter(TimeSpan interval, CancellationToken token)
        {
            _interval = interval;
            _externalToken = token;
        }

        public void Start()
        {
            _stopwatch.Start();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(_externalToken);
            var combinedToken = _cts.Token;
            _reportTask = Task.Run(async () =>
            {
                while (!combinedToken.IsCancellationRequested)
                {
                    await Task.Delay(_interval, combinedToken);

                    long filesWritten = Volatile.Read(ref _filesWritten);
                    double elapsed = _stopwatch.Elapsed.TotalSeconds;
                    double rate = elapsed > 0 ? filesWritten / elapsed : 0;

                    Console.WriteLine($"Воркеров: {_activeWorckersCount}, 📁 Папок: {Volatile.Read(ref _foldersProcessed)}, 📄 Файлов: {filesWritten}, ⏱️ {rate:F1} файлов/сек");
                }
            }, combinedToken);
        }

        public void SetActiveWorckersCount(int count) => _activeWorckersCount = count;
        public void IncrementFile() => Interlocked.Increment(ref _filesWritten);
        public void IncrementFolder() => Interlocked.Increment(ref _foldersProcessed);

        public async Task StopAsync()
        {
            _stopwatch.Stop();


            if (_cts is not null)
            {
                _cts.Cancel();
            }

            if (_reportTask is not null)
            {
                await _reportTask;
            }
            Console.WriteLine($"\n✅ Индексирование завершено: {_filesWritten} файлов за {_stopwatch.Elapsed.TotalMinutes:F1} мин");
        }
    }
}