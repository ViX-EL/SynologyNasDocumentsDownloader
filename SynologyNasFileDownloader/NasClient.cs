using Newtonsoft.Json.Linq;
using SynologyNas.Containers;
using SynologyNas.Search;
using SynologyNas.FilesIndex;

namespace SynologyNas
{
    public class NasClient : IDisposable
    {
        private ServiceApiContainer _servicesContainer;

        public bool IsAuthorized => _servicesContainer.Authorizator.IsAuthorized;

        public NasClient(string nasUrl, IFileSearcherFactory? fileSearcherFactory = null)
        {
            IFileSearcherFactory searcherFactory = fileSearcherFactory ?? new FileSearcherFactory();
            _servicesContainer = new(nasUrl, searcherFactory);
        }

        public async void Dispose()
        {
            await LogoutAsync();
        }

        public void ChangeUrl(string nasUrl)
        {
            _servicesContainer.Authorizator.ChangeUrl(nasUrl);
        }

        public async Task<bool> TryLoginAsync(string username, string password)
        {
            if (!IsAuthorized)
            {
                return await _servicesContainer.Authorizator.TryLoginAsync(username, password);
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _servicesContainer.Authorizator.LogoutAsync();
        }

        public async Task<Dictionary<string, string>?> SearchPdfFilesAsync(string nasFolderPath, HashSet<string> fileNamesWithoutExtension)
        {
            if (!IsAuthorized)
            {
                Console.WriteLine($"Поиск pdf файлов не возможен Sid Synology Nas не был получен!");
                return null;
            }
            return await _servicesContainer.Pdf_File_Searcher.SearchAsync(nasFolderPath, fileNamesWithoutExtension);
        }
        public async Task<Dictionary<string, List<string>>?> SearchSigFilesAsync(string nasFolderPath, HashSet<string> fileNamesWithoutExtension)
        {
            if (!IsAuthorized)
            {
                Console.WriteLine($"Поиск sig файлов не возможен Sid Synology Nas не был получен!");
                return null;
            }
            return await _servicesContainer.Sig_File_Searcher.SearchAsync(nasFolderPath, fileNamesWithoutExtension);
        }

        public async Task<List<string>> DownloadFilesAsync(HashSet<string> filePaths, string localSavePath = "")
        {
            if (!IsAuthorized)
            {
                Console.WriteLine("Скачивание pdf файлов не возможно, Sid Synology Nas не был получен!");
                return new();
            }
            return await _servicesContainer.File_Downloader.DownloadAsync(filePaths, localSavePath);
        }

        public async Task<string?> GetApiInfoAsync()
        {
            return await _servicesContainer.ApiInfo.GetApiInfoAsync();
        }

        public async Task<string?> GetListSharedFoldersAsync()
        {
            if (!IsAuthorized)
            {
                Console.WriteLine("Получение списка доступных корневых папок не возможно, Sid Synology Nas не был получен!");
                return null;
            }
            return await _servicesContainer.List.ListSharedFoldersAsync();
        }

        public async Task<(List<JToken>, bool)> ListFolderAsync(string folderPath, int limit, int offset, CancellationToken token)
        {
            if (!IsAuthorized)
            {
                Console.WriteLine("Получение списка файлов в папке не возможно, Sid Synology Nas не был получен!");
                return (new(), false);
            }
            return await _servicesContainer.List.ListFolderAsync(folderPath, limit, offset, token);
        }

        public async Task IndexFolderAsync(string nasFolderPath, string localSaveFolderPath = "")
        {
            if (!IsAuthorized)
            {
                Console.WriteLine("Рекурсивное получение списка файлов в папке не возможно, Sid Synology Nas не был получен!");
                return;
            }

            try
            {
                CancellationToken cancellationToken = new();
                FileIndexProgressReporter progressReporter = new(TimeSpan.FromSeconds(5), cancellationToken);
                RotatingJSONLIndexWriter indexWriter = new(localSaveFolderPath);
                const int workerCapacity = 1000;
                IndexWriterWorker indexWriterWorker = new IndexWriterWorker(indexWriter, workerCapacity, cancellationToken);
                FolderIndexer indexer = new(_servicesContainer.List, indexWriterWorker, progressReporter, 5);
                await indexer.IndexAsync(nasFolderPath, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}", ex);
            }
        }
    }
}