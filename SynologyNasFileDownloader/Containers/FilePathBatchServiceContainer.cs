using SynologyNas.Batch;

namespace SynologyNas.Containers
{
    public class FilePathBatchServiceContainer
    {
        public readonly StringBatcher Batcher;
        public readonly FilePathBatchPreparer BatchPreparer;
        public FilePathBatchServiceContainer()
        {
            Batcher = new();
            BatchPreparer = new();
        }
    }
}