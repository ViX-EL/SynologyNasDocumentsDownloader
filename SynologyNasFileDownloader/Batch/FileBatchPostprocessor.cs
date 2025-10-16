namespace SynologyNas.Batch
{
    public class FileNameBatchPostprocessor
    {
        public List<List<string>> AddFileNamesWithSpacesToBatches(List<List<string>> batches, HashSet<string> fileNamesWithSpaces)
        {
            foreach (var fileName in fileNamesWithSpaces)
            {
                batches.Add(new List<string> { fileName });
            }
            return batches;
        }
    }
}