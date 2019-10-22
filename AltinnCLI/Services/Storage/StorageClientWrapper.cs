using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Services.Storage
{
    public class StorageClientWrapper : IStorageClientWrapper
    {
        public StorageClientWrapper()
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("AppAPIBaseAddress").Get<string>();
        }

        private string BaseAddress { get; set; }

        public string CreateApplication(string org, string app, string instanceOwnerId, HttpContent content)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("AppAPIBaseAddress").Get<string>().Replace("{org}", org);
            string cmd = $@"{org}/{app}/instances";
            
            HttpClientWrapper httpClientWrapper = new HttpClientWrapper();

            Task<HttpResponseMessage> response = httpClientWrapper.PostCommand(BaseAddress, cmd, content);

            return response.Result.Content.ReadAsStringAsync().Result;

        }

        public string CreateApplication(string appId, string instanceOwnerId, StringContent content)
        {
            throw new NotImplementedException();
        }

        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            string cmd = $@"instances/{instanceOwnerId}/{instanceGuid}/data/{dataId}";

            HttpClientWrapper httpClinetWrapper = new HttpClientWrapper();

            Task<HttpResponseMessage> response = httpClinetWrapper.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }

        public Stream GetDocument(string command, string contentType = null)
        {

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper();
            Uri uri = new Uri(command);

            HttpResponseMessage response = (HttpResponseMessage)httpClientWrapper.GetWithUrl(uri, contentType).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) || contentType == null)
                {
                    return response.Content.ReadAsStreamAsync().Result;
                }
                else if (contentType.Contains("application/pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return response.Content.ReadAsStreamAsync().Result;
                }

                return null;
            }
            else
            {
                return null;
            }
        }

        public InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId=null, Guid? instanceGuid = null)
        {
            string cmd = "instances";

            if (instanceOwnerId != null)
            {
                cmd += $@"?instanceOwnerId={instanceOwnerId}";

                if (instanceGuid != null)
                {
                    cmd += $@"\{instanceGuid}";
                }
            }

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public InstanceResponseMessage GetInstanceMetaData(Uri uri)
        {

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetWithUrl(uri);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public Stream GetInstances(int instanceOwnerId, Guid instanceGuid)
        {
            string cmd = "instances";

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }


    }
}
