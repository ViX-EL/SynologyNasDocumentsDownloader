using Newtonsoft.Json.Linq;

namespace SynologyNas.Api
{
    public class SearchApiService
    {
        private readonly AuthorizeApiService _authorizator;

        public SearchApiService(AuthorizeApiService authorizator)
        {
            _authorizator = authorizator;
        }

        public async Task<string?> StartSearchAsync(string folderPath, string pattern, string extension)
        {
            string encodedPattern = Uri.EscapeDataString(pattern);
            string encodedFolderPath = Uri.EscapeDataString($"[\"{folderPath}\"]");
            string encodedFileType = Uri.EscapeDataString("\"file\"");
            var searchUrl = $"{_authorizator.NasUrl}/webapi/entry.cgi?api=SYNO.FileStation.Search" +
                "&version=2" +
                "&method=start" +
                $"&folder_path={encodedFolderPath}" +
                "&recursive=true" +
                $"&filetype={encodedFileType}" +
                $"&pattern={encodedPattern}" +
                $"&extension={extension}" +
                "&search_content=false" +
                $"&_sid={_authorizator.Sid}";
            var response = await _authorizator.Client.GetStringAsync(searchUrl);
            string? taskId = JObject.Parse(response)["data"]?["taskid"]?.ToString();

            if (taskId == null)
            {
                Console.WriteLine("taskId не был получен!");
            }

            return taskId;
        }

        public async Task<List<JToken>> GetSearchResultsAsync(string taskId)
        {
            var allFiles = new List<JToken>();

            while (true)
            {
                string encodedFileType = Uri.EscapeDataString("\"file\"");
                string encodedTaskId = Uri.EscapeDataString($"\"{taskId}\"");
                var listUrl = $"{_authorizator.NasUrl}/webapi/entry.cgi?" +
                    "api=SYNO.FileStation.Search" +
                    "&version=2" +
                    "&method=list" +
                    $"&taskid={encodedTaskId}" +
                    "&limit=-1" +
                    $"&filetype={encodedFileType}" +
                    $"&_sid={_authorizator.Sid}";

                string listResponse = await _authorizator.Client.GetStringAsync(listUrl);
                var parsed = JObject.Parse(listResponse);

                var files = parsed["data"]?["files"] as JArray;
                if (files != null && files.Count > 0)
                    allFiles.AddRange(files);

                bool finished = parsed["data"]?["finished"]?.Value<bool>() ?? false;
                if (finished)
                    break;

                await Task.Delay(1000);
            }

            return allFiles;
        }

        private async Task<string> StopSearchAsync(string taskId)
        {
            var resultUrl = $"{_authorizator.NasUrl}/webapi/entry.cgi?" +
            "api=SYNO.FileStation.Search" +
            "&version=2" +
            "&method=stop" +
            $"&taskid={taskId}" +
            $"&_sid={_authorizator.Sid}";
            return await _authorizator.Client.GetStringAsync(resultUrl);
        }

        private async Task<string> CleanSearchAsync(string taskId)
        {
            var resultUrl = $"{_authorizator.NasUrl}/webapi/entry.cgi?" +
            "api=SYNO.FileStation.Search" +
            "&version=2" +
            "&method=clean" +
            $"&taskid={taskId}" +
            $"&_sid={_authorizator.Sid}";
            return await _authorizator.Client.GetStringAsync(resultUrl);
        }

        public async Task StopAndCleanSearchAsync(string taskId)
        {
            await StopSearchAsync(taskId);
            await CleanSearchAsync(taskId);
        }
    }
}
