using SynologyNas.Api;
using SynologyNas.Containers;
using Gma.DataStructures.StringSearch;

namespace SynologyNas.Search
{
    public class FileSearcher : IFileSearcher
    {
        private SearchServiceContainer _serviceContainer;
        private SearchApiService _searchApiService;

        public FileSearcher(AuthorizeApiService authorizator)
        {
            _serviceContainer = new();
            _searchApiService = new(authorizator);
        }

        public async Task<Trie<string>?> SearchAsync(string folderPath, HashSet<string> fileNames, string extension)
        {
            List<string> patternBatches = _serviceContainer.PatternCreator.CreatePatternBatches(fileNames, 50);

            string[]? taskIds = await _serviceContainer.TaskManager.AccumulateSearchTasks(folderPath, patternBatches, extension, _searchApiService);
            if (taskIds == null)
            {
                return null;
            }

            Trie<string> results = await _serviceContainer.SearchResultsManager.AccumulateSearchResults(taskIds, _searchApiService);

            foreach (var taskId in taskIds)
            {
                await _searchApiService.StopAndCleanSearchAsync(taskId);
            }

            return results;
        }
    }
}