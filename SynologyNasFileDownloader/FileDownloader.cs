using SynologyNas.Api;
using SynologyNas.Containers;

namespace SynologyNas
{
    public class FileDownloader
    {
        private readonly FilePathServiceContainer _serviceContainer;
        private readonly DownloadApiService _downloadApiClient;

        public FileDownloader(AuthorizeApiService authorizator)
        {
            _serviceContainer = new();
            _downloadApiClient = new DownloadApiService(authorizator);
        }

        public async Task<List<string>> DownloadAsync(HashSet<string> nasFilePaths, string localSavePath = "")
        {
            var downloadedFiles = new List<string>();

            if (nasFilePaths.Count == 0)
            {
                return downloadedFiles;
            }

            await _serviceContainer.fileSystemManager.EnsureDirectoryExistsAsync(localSavePath);

            const int maxBatchSize = 20;
            List<string> batches = _serviceContainer.batchCreator.CreateBatches(nasFilePaths, maxBatchSize);
            foreach (string pathsBatch in batches)
            {
                using (Stream stream = await _downloadApiClient.DownloadFilesAsync(pathsBatch))
                {
                    if (pathsBatch.Contains(","))
                    {
                        var extracted = await _serviceContainer.fileSystemManager.ExtractArchiveFromStreamAsync(stream, localSavePath);
                        downloadedFiles.AddRange(extracted);
                    }
                    else
                    {
                        var fileName = Path.GetFileName(pathsBatch.Replace("\"", ""));
                        var savedFilePath = await _serviceContainer.fileSystemManager.SaveFileFromStreamAsync(stream, localSavePath, fileName);
                        downloadedFiles.Add(savedFilePath);
                    }
                }
            }
            return downloadedFiles;
        }
    }
}