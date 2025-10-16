using SynologyNas.Api;

namespace SynologyNas.Search
{
    public class FileSearcherFactory : IFileSearcherFactory
    {
        public IFileSearcher Create(AuthorizeApiService authorizator)
        {
            return new FileSearcher(authorizator);
        }
    }
}