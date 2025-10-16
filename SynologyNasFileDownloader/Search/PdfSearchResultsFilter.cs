using Gma.DataStructures.StringSearch;
using System.Text.RegularExpressions;

namespace SynologyNas.Search
{
    public class PdfSearchResultsFilter
    {
        public Dictionary<string, string> Filter(Trie<string> pdfSearchResults, HashSet<string> fileNamesWithoutExtension)
        {
            Dictionary<string, string> filteredPdfFiles = new();
            foreach (var fileName in fileNamesWithoutExtension)
            {
                IEnumerable<string> pdfFilesForName = pdfSearchResults.Retrieve(fileName);
                string? fileWithMaxNumber = pdfFilesForName.OrderByDescending(file =>
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    Regex regex = new Regex(@$"{fileName}(?:_signed)? ?(?:\((\d+)\))?(?:\.[\w\.]*)?");
                    Match match = regex.Match(fileNameWithoutExtension);
                    if (match.Success && match.Groups[1].Value != "")
                    {
                        return int.Parse(match.Groups[1].Value);
                    }
                    return 0;
                })
                .FirstOrDefault();

                if (fileWithMaxNumber != null)
                {
                    filteredPdfFiles.Add(fileName, fileWithMaxNumber);
                }
            }
            return filteredPdfFiles;
        }
    }
}