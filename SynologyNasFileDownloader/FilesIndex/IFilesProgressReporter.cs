
namespace SynologyNas.FilesIndex
{
    public interface IFilesProgressReporter
    {
        void IncrementFile();
        void IncrementFolder();
        void Start();
        Task StopAsync();
        void SetActiveWorckersCount(int count);
    }
}