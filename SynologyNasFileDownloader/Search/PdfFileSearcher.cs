using Gma.DataStructures.StringSearch;

namespace SynologyNas.Search
{
    public class PdfFileSearcher
    {
        private readonly IFileSearcher _fileSearcher;
        private readonly PdfSearchResultsFilter _searchFilter;

        public PdfFileSearcher(IFileSearcher searcher, PdfSearchResultsFilter searchFilter)
        {
            _fileSearcher = searcher;
            _searchFilter = searchFilter;
        }

        public async Task<Dictionary<string, string>?> SearchAsync(string targetFolder, HashSet<string> documentNames)
        {
            Trie<string>? searchResults = await _fileSearcher.SearchAsync(targetFolder, documentNames, "pdf");
            if (searchResults != null)
            {
                return _searchFilter.Filter(searchResults, documentNames);
            }
            return null;
        }
    }
}