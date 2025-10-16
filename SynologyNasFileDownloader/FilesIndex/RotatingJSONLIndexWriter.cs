using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SynologyNas.FilesIndex
{
    public sealed class RotatingJSONLIndexWriter : IIndexWriter
    {
        private readonly string _directory;
        private readonly string _prefix;
        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private StreamWriter? _writer;
        private DateTime _currentDate;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RotatingJSONLIndexWriter(string directory, string prefix = "index")
        {
            _directory = directory;
            _prefix = prefix;
            Directory.CreateDirectory(directory);

            _currentDate = DateTime.UtcNow.Date;
            OpenWriterForDate(_currentDate);
        }

        private void OpenWriterForDate(DateTime date)
        {
            string fileName = $"{_prefix}_{date:yyyy-MM-dd}.jsonl";
            string filePath = Path.Combine(_directory, fileName);

            _writer?.Dispose();
            _writer = new StreamWriter(
                new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
            )
            {
                AutoFlush = false
            };
        }

        private void CheckForRotation()
        {
            var now = DateTime.UtcNow.Date;
            if (now > _currentDate)
            {
                _currentDate = now;
                OpenWriterForDate(_currentDate);
            }
        }

        public async Task WriteAsync(FileEntry entry, CancellationToken token = default)
        {
            // var options = new JsonSerializerOptions
            // {
            //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //     WriteIndented = false
            // };

            // string json = JsonSerializer.Serialize(entry, options);

            string json = JsonSerializer.Serialize(entry, _jsonOptions);

            await _writeLock.WaitAsync(token);
            try
            {
                CheckForRotation();
                await _writer!.WriteLineAsync(json);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task FlushAsync(CancellationToken token = default)
        {
            await _writeLock.WaitAsync(token);
            try
            {
                await _writer!.FlushAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _writeLock.WaitAsync();
            try
            {
                await _writer!.FlushAsync();
                _writer.Dispose();
            }
            finally
            {
                _writeLock.Release();
                _writeLock.Dispose();
            }
        }
    }
}
