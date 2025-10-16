using System.Diagnostics;

namespace SynologyNas.FilesIndex
{
    public class SilentProgressReporter : IFilesProgressReporter
    {
        private long _filesWritten = 0;
        private long _foldersProcessed = 0;
        private int _activeWorckersCount = 0;
        private readonly Stopwatch _stopwatch = new();

        public long Files => _filesWritten;
        public long Folders => _foldersProcessed;
        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public void Start() => _stopwatch.Start();

        public void IncrementFile() => Interlocked.Increment(ref _filesWritten);
        public void IncrementFolder() => Interlocked.Increment(ref _foldersProcessed);
        public void SetActiveWorckersCount(int count) => _activeWorckersCount = count;


        public Task StopAsync()
        {
            _stopwatch.Stop();
            return Task.CompletedTask;
        }

        public override string ToString() =>
            $"Воркеров: {_activeWorckersCount}, 📁 Папок: {_foldersProcessed}, 📄 Файлов: {_filesWritten}, Время: {_stopwatch.Elapsed.TotalSeconds:F1} сек";
    }
}