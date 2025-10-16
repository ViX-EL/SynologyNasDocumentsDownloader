using SynologyNas.Batch;

namespace SynologyNas.Containers
{
    public class FileNameBatchServiceContainer
    {
        public readonly FileSpacesFilter FileNameFilter;
        public readonly StringBatcher FileBatcher;
        public readonly FileNameBatchPostprocessor FileBatchPostprocessor;
        public readonly FileNameBatchPreparer FileBatchPreparer;

        public FileNameBatchServiceContainer()
        {
            FileNameFilter = new();
            FileBatcher = new();
            FileBatchPostprocessor = new();
            FileBatchPreparer = new();
        }
    }
}