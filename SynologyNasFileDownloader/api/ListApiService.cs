using Newtonsoft.Json.Linq;

namespace SynologyNas.Api
{
    public class ListApiService
    {
        private readonly Lazy<HttpClient> _client;
        private readonly AuthorizeApiService _authorizator;

        public ListApiService(AuthorizeApiService authorizator, TimeSpan timeout)
        {
            _authorizator = authorizator;
            _client = new Lazy<HttpClient>(() =>
            {
                var client = new HttpClient();
                client.Timeout = timeout;
                return client;
            });
        }
        
        public async Task<string?> ListSharedFoldersAsync()
        {
            var checkUrl = $"{_authorizator.NasUrl}/webapi/entry.cgi?" +
            "api=SYNO.FileStation.List" +
            "&version=2" +
            "&method=list_share" +
            $"&_sid={_authorizator.Sid}";
            var response = await _authorizator.Client.GetStringAsync(checkUrl);
            return JObject.Parse(response).ToString();
        }

        public async Task<(List<JToken>, bool)> ListFolderAsync(string folderPath, int limit, int offset, CancellationToken token)
        {
            string encodedFolderPath = Uri.EscapeDataString(folderPath);
            string url = $"{_authorizator.NasUrl}/webapi/entry.cgi?" +
                        "api=SYNO.FileStation.List" +
                        "&version=2" +
                        "&method=list" +
                        $"&folder_path={encodedFolderPath}" +
                        "&filetype=all" +
                        "&additional=[\"size\",\"owner\",\"time\",\"type\"]" +
                        $"&limit={limit}" +
                        $"&offset={offset}" +
                        $"&_sid={_authorizator.Sid}";

            using var response = await _client.Value.SendAsync(new HttpRequestMessage(HttpMethod.Get, url),
            HttpCompletionOption.ResponseHeadersRead, token);

            string? responceStr = await response.Content.ReadAsStringAsync();
            if(responceStr == null)
            {
                throw new Exception($"Ответ на запрос {url} не был получен!");
            }
            var parsed = JObject.Parse(responceStr);
            
            var allItems = new List<JToken>();
            var filesAndDirs = parsed["data"]?["files"] as JArray;
            if (filesAndDirs != null && filesAndDirs.Count > 0)
            {
                allItems.AddRange(filesAndDirs);
            }

            bool isSearchFinished = false;
            if(allItems.Count < limit)
            {
                isSearchFinished = true;
            }

            return (allItems, isSearchFinished);
        }
    }
}
