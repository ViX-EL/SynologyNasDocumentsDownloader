using SynologyNas.Containers;

namespace SynologyNas.Batch
{
    public class FileNamesBatchPatternCreator
    {
        private FileNameBatchServiceContainer _serviceContainer;

        public FileNamesBatchPatternCreator(FileNameBatchServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

        public List<string> CreatePatternBatches(HashSet<string> fileNames, int batchSize)
        {
            var fileNamesWithSpaces = _serviceContainer.FileNameFilter.GetFileNamesWithSpaces(fileNames);
            var fileNamesWithoutSpaces = _serviceContainer.FileNameFilter.GetFileNamesWithoutSpaces(fileNames, fileNamesWithSpaces);
            var batches = _serviceContainer.FileBatcher.CreateBatches(fileNamesWithoutSpaces, batchSize);
            batches = _serviceContainer.FileBatchPostprocessor.AddFileNamesWithSpacesToBatches(batches, fileNamesWithSpaces);
            List<string> patternBatches = new();
            foreach (var batch in batches)
            {
                patternBatches.Add(_serviceContainer.FileBatchPreparer.PrepareBatch(batch));
            }
            return patternBatches;
        }
    }
}