using Newtonsoft.Json.Linq;

namespace SynologyNas.Api
{
    public class AuthorizeApiService
    {
        public HttpClient Client { get; private set; }
        public string NasUrl { get; private set; }
        public string Sid { get; private set; } = "";

        public bool IsAuthorized => Sid != "";

        public AuthorizeApiService(HttpClient client, string nasUrl)
        {
            Client = client;
            NasUrl = nasUrl;
        }

        public async Task LoginAsync(string username, string password)
        {
            string encodedUsername = Uri.EscapeDataString(username);
            string encodedPassword = Uri.EscapeDataString(password);
            var authUrl = $"{NasUrl}/webapi/entry.cgi?" +
                "api=SYNO.API.Auth" +
                "&version=7" +
                "&method=login" +
                $"&account={encodedUsername}" +
                $"&passwd={encodedPassword}" +
                "&session=FileStation" +
                "&format=sid";
            var response = await Client.GetStringAsync(authUrl);

            string? success = JObject.Parse(response)["success"]?.ToString();
            if (success != null && success == "False")
            {
                string? error = JObject.Parse(response)["error"]?["code"]?.ToString();
                if (error != null && error == "400")
                {
                    Console.WriteLine("Неверный логин или пароль, указанный для сетевого диска!");
                }
                return;
            }

            Sid = JObject.Parse(response)["data"]?["sid"]?.ToString() ?? "";

            if (Sid == "")
            {
                Console.WriteLine("Sid Synology Nas не был получен!");
            }
        }

        public async Task<string> LogoutAsync()
        {
            if(!IsAuthorized)
            {
                return "";
            }
            var resultUrl = $"{NasUrl}/webapi/auth.cgi?" +
                "api=SYNO.API.Auth" +
                "&version=1" +
                "&method=logout" +
                "&session=FileStation" +
                $"&_sid={Sid}";
            return await Client.GetStringAsync(resultUrl);
        }
    }
}