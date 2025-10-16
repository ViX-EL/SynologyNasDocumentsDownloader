using SynologyNas.Api;
using SynologyNas.Search;

namespace SynologyNas.Containers
{
    public class ServiceApiContainer
    {
        public readonly HttpClient Client = new HttpClient();
        private readonly IFileSearcher _fileSearcher;
        public readonly AuthorizeApiService Authorizator;
        public readonly FileDownloader File_Downloader;
        public readonly ListApiService List;
        public readonly PdfFileSearcher Pdf_File_Searcher;
        public readonly SigFileSearcher Sig_File_Searcher;
        public readonly InfoApiService ApiInfo;

        public ServiceApiContainer(string nasUrl, IFileSearcherFactory fileSearcherFactory)
        {
            Authorizator = new(Client, nasUrl);
            File_Downloader = new(Authorizator);
            _fileSearcher = fileSearcherFactory.Create(Authorizator);
            Pdf_File_Searcher = new(_fileSearcher, new());
            Sig_File_Searcher = new(_fileSearcher, new());
            ApiInfo = new(Authorizator);
            TimeSpan listRequestTimeout = TimeSpan.FromMinutes(15);
            List = new(Authorizator, listRequestTimeout);
        }
    }
}