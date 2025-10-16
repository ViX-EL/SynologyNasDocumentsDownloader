using SynologyNas.Containers;

namespace SynologyNas.Batch
{
    public class FilePathsBatchCreator
    {
        private readonly FilePathBatchServiceContainer _serviceContainer;

        public FilePathsBatchCreator(FilePathBatchServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

        public List<string> CreateBatches(HashSet<string> filePaths, int batchSize)
        {
            var batches = _serviceContainer.Batcher.CreateBatches(filePaths, batchSize);
            List<string> outBatches = new();
            foreach (var batch in batches)
            {
                outBatches.Add(_serviceContainer.BatchPreparer.PrepareBatch(batch));
            }
            return outBatches;
        }
    }
}