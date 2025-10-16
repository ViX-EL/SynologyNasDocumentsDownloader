using System.Threading.Channels;

namespace SynologyNas.FilesIndex
{
    public sealed class IndexWriterWorker : IAsyncDisposable
    {
        private readonly Channel<FileEntry> _channel;
        private readonly IIndexWriter _writer;
        private readonly CancellationToken _token;
        private readonly Task _consumer;

        public IndexWriterWorker(IIndexWriter writer, int capacity, CancellationToken token)
        {
            _writer = writer;
            _token = token;

            _channel = Channel.CreateBounded<FileEntry>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

            _consumer = Task.Run(ConsumeAsync, _token);
        }

        public async Task EnqueueAsync(FileEntry entry)
        {
            await _channel.Writer.WriteAsync(entry, _token);
        }

        private async Task ConsumeAsync()
        {
            try
            {
                await foreach (var entry in _channel.Reader.ReadAllAsync(_token))
                {
                    await _writer.WriteAsync(entry, _token);
                }
            }
            catch (OperationCanceledException) { }
        }

        public async ValueTask DisposeAsync()
        {
            _channel.Writer.Complete();
            await _consumer;
            await _writer.FlushAsync(_token);
            await _writer.DisposeAsync();
        }
    }
}