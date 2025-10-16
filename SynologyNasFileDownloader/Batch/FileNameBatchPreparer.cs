namespace SynologyNas.Batch
{
    public class FileNameBatchPreparer
    {
        public string PrepareBatch(List<string> fileNamesBatch)
        {
            // if(fileNamesBatch.Count == 1)
            // {
            //     fileNamesBatch[0] = fileNamesBatch.First().Replace(" ", " AND ");
            // }
            return $"\"{string.Join(" OR ", fileNamesBatch)}\"";
        }
    }
}