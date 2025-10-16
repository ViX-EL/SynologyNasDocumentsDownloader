using Gma.DataStructures.StringSearch;

namespace SynologyNas.Search
{
    public class SigFileSearcher
    {
        private readonly IFileSearcher _fileSearcher;
        private readonly SigSearchResultsFilter _searchFilter;

        public SigFileSearcher(IFileSearcher searcher, SigSearchResultsFilter searchFilter)
        {
            _fileSearcher = searcher;
            _searchFilter = searchFilter;
        }

        public async Task<Dictionary<string, List<string>>?> SearchAsync(string targetFolder, HashSet<string> fileNamesWithoutExtension)
        {
            Trie<string>? searchResults = await _fileSearcher.SearchAsync(targetFolder, fileNamesWithoutExtension, "pdf");
            if (searchResults != null)
            {
                return _searchFilter.Filter(searchResults, fileNamesWithoutExtension);
            }
            return null;
        }
    }
}