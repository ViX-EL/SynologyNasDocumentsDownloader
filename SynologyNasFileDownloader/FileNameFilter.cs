namespace SynologyNas
{
    public class FileSpacesFilter
    {
        public HashSet<string> GetFileNamesWithSpaces(HashSet<string> fileNames)
        {
            return fileNames.Where(x => x.Contains(" ")).ToHashSet();
        }

        public HashSet<string> GetFileNamesWithoutSpaces(HashSet<string> fileNames, HashSet<string> fileNamesWithSpaces)
        {
            return fileNames.Except(fileNamesWithSpaces).ToHashSet();
        }
    }
}