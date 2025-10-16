namespace SynologyNas.Batch
{
    public class FilePathBatchPreparer
    {
        public string PrepareBatch(List<string> filePathsBatch)
        {
            return string.Join(",", filePathsBatch.Select(path => $"\"{path}\""));
        }
    }
}