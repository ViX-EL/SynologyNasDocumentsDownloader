using Newtonsoft.Json.Linq;

namespace SynologyNas.Api
{
    public class InfoApiService
    {
        private readonly AuthorizeApiService _authorizator;

        public InfoApiService(AuthorizeApiService authorizator)
        {
            _authorizator = authorizator;
        }

        public async Task<string?> GetApiInfoAsync()
        {
            var authUrl = $"{_authorizator.NasUrl}/webapi/query.cgi?api=SYNO.API.Info&version=1&method=query&query=all";
            var response = await _authorizator.Client.GetStringAsync(authUrl);
            string? data = JObject.Parse(response)["data"]?.ToString();

            if (data == null)
            {
                Console.WriteLine("Info не было получено!");
            }

            return data;
        }
    }
}