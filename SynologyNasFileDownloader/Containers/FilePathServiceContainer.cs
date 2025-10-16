using SynologyNas.Batch;

namespace SynologyNas.Containers
{
    public class FilePathServiceContainer
    {
        public readonly FilePathsBatchCreator batchCreator;
        public readonly FileSystemManager fileSystemManager;

        public FilePathServiceContainer()
        {
            batchCreator = new(new FilePathBatchServiceContainer());
            fileSystemManager = new();
        }
    }
}