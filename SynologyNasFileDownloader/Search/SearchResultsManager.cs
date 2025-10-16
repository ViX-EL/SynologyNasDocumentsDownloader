using Newtonsoft.Json.Linq;
using SynologyNas.Api;
using Gma.DataStructures.StringSearch;

namespace SynologyNas.Search
{
    public class ResultsManager
    {
        public async Task<Trie<string>> AccumulateSearchResults(string[] taskIds, SearchApiService searchClient)
        {
            var results = new List<JToken>();
            foreach (string taskId in taskIds)
            {
                results.AddRange(await searchClient.GetSearchResultsAsync(taskId));
            }

            Trie<string> resultsTrie = new();
            foreach (var result in results)
            {
                string? nasFilePath = result["path"]?.ToString();
                string? fileNameWithoutExtension = Path.GetFileNameWithoutExtension(result["name"]?.ToString());
                if (nasFilePath == null || fileNameWithoutExtension == null)
                {
                    continue;
                }
                resultsTrie.Add(fileNameWithoutExtension, nasFilePath);
            }
            return resultsTrie;
        }
    }
}