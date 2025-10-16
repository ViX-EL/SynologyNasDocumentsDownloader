
namespace SynologyNas.Api
{
    public class DownloadApiService
    {
        private readonly AuthorizeApiService _authorizator;

        public DownloadApiService(AuthorizeApiService authorizator)
        {
            _authorizator = authorizator;
        }

        public async Task<Stream> DownloadFilesAsync(string nasFilePaths)
        {
            string encodedNasFilePaths = Uri.EscapeDataString($"[{nasFilePaths}]");
            var url = $"{_authorizator.NasUrl}/webapi/entry.cgi" +
                $"?api=SYNO.FileStation.Download" +
                $"&version=2" +
                $"&method=download" +
                $"&path={encodedNasFilePaths}" +
                $"&mode=download" +
                $"&_sid={_authorizator.Sid}";

            var resp = await _authorizator.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadAsStreamAsync();
        }
    }
}