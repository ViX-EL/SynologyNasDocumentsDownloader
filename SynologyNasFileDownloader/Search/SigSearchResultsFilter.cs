using Gma.DataStructures.StringSearch;
using System.Text.RegularExpressions;

namespace SynologyNas.Search
{
    public class SigSearchResultsFilter
    {
        public Dictionary<string, List<string>> Filter(Trie<string> sigSearchResults, HashSet<string> fileNamesWithoutExtension)
        {
            Dictionary<string, List<string>> filteredSigFiles = new();
            foreach (var fileName in fileNamesWithoutExtension)
            {
                Regex regex = new Regex(@$"{fileName}(?: ?\(\d+\))?\.");
                IEnumerable<string> sigFilesForName = sigSearchResults.Retrieve(fileName);
                foreach (string filePath in sigFilesForName)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    Match match = regex.Match(fileNameWithoutExtension);
                    if (match.Success)
                    {
                        if (!filteredSigFiles.ContainsKey(fileName))
                        {
                            filteredSigFiles.Add(fileName, new List<string>());
                        }
                        filteredSigFiles[fileName].Add(filePath);
                    }
                }
            }
            return filteredSigFiles;
        }
    }
}