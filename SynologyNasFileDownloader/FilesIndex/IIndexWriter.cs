
namespace SynologyNas.FilesIndex
{
    public sealed class FileEntry
    {
        public string Path { get; init; } = "";
        public string Name { get; init; } = "";
        public string Owner { get; init; } = "";
        public int Size { get; init; }
        public DateTime Modified { get; init; }
        public string Extension { get; init; } = "";
        public bool IsDirectory { get; init; }
    }

    public interface IIndexWriter : IAsyncDisposable
    {
        Task WriteAsync(FileEntry entry, CancellationToken token = default);
        Task FlushAsync(CancellationToken token = default);
    }
}