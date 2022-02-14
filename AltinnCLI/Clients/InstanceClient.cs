using Altinn.Platform.Storage.Interface.Models;

using AltinnCLI.Commands.Core;
using AltinnCLI.Helpers;
using AltinnCLI.Models;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("AppBaseAddress").Get<string>().Replace("{org}", org);
            requestUri += $@"/{org}/{app}/instances";

            HttpResponseMessage response = await _client.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not create application instance. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<InstanceResponseMessage> GetAllAppInstances(string org, string app)
        {
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("StorageBaseAddress").Get<string>();
            requestUri += $@"/instances?appId={org}/{app}";

            HttpResponseMessage response = await _client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not retrieve application instances. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InstanceResponseMessage>(responseString, Globals.JsonSerializerOptions);
        }

        public async Task<Stream> GetInstanceStream(int instanceOwnerId, Guid instanceGuid)
        {
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("StorageBaseAddress").Get<string>();

            requestUri += $@"/instances/{instanceOwnerId}/{instanceGuid}";

            HttpResponseMessage response = await _client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not retrieve application instance. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            Stream stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Instance> GetInstance(int instanceOwnerId, Guid instanceGuid)
        {
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("StorageBaseAddress").Get<string>();
            requestUri += $"/instances/{instanceOwnerId}/{instanceGuid}";


            HttpResponseMessage response = await _client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not retrieve instance {instanceOwnerId}/{instanceGuid}. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            // return instanse not response msg
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Instance>(responseString, Globals.JsonSerializerOptions);
          
        }

        public async Task<InstanceResponseMessage> GetInstances(List<IOption> urlParams)
        {
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("StorageBaseAddress").Get<string>();
            requestUri += "/instances?";

            foreach (IOption param in urlParams)
            {
                if (param.IsAssigned == true && !string.IsNullOrEmpty(param.ApiName))
                {
                    requestUri += $@"&{param.ApiName}={param.Value}";
                }
            }

            HttpResponseMessage response = await _client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not retrieve application instances. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }


            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InstanceResponseMessage>(responseString, Globals.JsonSerializerOptions);
        }

        public async Task<InstanceResponseMessage> GetInstances(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Could not retrieve application instances. Error code: {response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InstanceResponseMessage>(responseString, Globals.JsonSerializerOptions);
        }
    }
}
