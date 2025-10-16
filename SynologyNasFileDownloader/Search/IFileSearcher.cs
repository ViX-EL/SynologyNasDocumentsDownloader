using Gma.DataStructures.StringSearch;

namespace SynologyNas.Search
{
    public interface IFileSearcher
    {
        Task<Trie<string>?> SearchAsync(string folderPath, HashSet<string> documentNames, string extension);
    }
}