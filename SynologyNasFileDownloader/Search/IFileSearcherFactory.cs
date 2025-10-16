using SynologyNas.Api;

namespace SynologyNas.Search
{
    public interface IFileSearcherFactory
    {
        IFileSearcher Create(AuthorizeApiService authorizator);
    }
}