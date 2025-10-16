using SynologyNas.Search;
using SynologyNas.Batch;

namespace SynologyNas.Containers
{
    public class SearchServiceContainer
    {
        private readonly FileNameBatchServiceContainer _batchServiceContainer;
        public readonly FileNamesBatchPatternCreator PatternCreator;
        public readonly TaskManager TaskManager;
        public readonly ResultsManager SearchResultsManager;

        public SearchServiceContainer()
        {
            _batchServiceContainer = new();
            PatternCreator = new(_batchServiceContainer);
            TaskManager = new();
            SearchResultsManager = new();
        }
    }
}