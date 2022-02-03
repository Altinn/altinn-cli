using Microsoft.Extensions.Configuration;

using System.Net.Http;
using System.Threading.Tasks;

namespace AltinnCLI.Clients
{
    public class InstanceClient
    {
        private readonly HttpClient _client;

        public InstanceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> PostInstance(string org, string app, HttpContent content)
        {
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("AppAPIBaseAddress").Get<string>().Replace("{org}", org);
            requestUri += $@"/{org}/{app}/instances";

            HttpResponseMessage response = await _client.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not create application instance. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
