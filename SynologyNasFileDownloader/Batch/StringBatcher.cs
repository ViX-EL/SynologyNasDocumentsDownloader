namespace SynologyNas.Batch
{
    public class StringBatcher
    {
        public List<List<string>> CreateBatches(HashSet<string> inString, int batchSize)
        {
            return inString
                .Select((name, index) => new { name, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.name).ToList())
                .ToList();
        }
    }
}