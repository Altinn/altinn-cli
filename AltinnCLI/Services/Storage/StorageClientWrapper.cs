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
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
        }

        private string BaseAddress { get; set; }

        public string CreateApplication(string appId, string instanceOwnerId, HttpContent content)
        {
            string cmd = $@"instances?appId={appId}&instanceOwnerId={instanceOwnerId}";

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

        public Stream GetDocument(string command)
        {

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper();
            Uri uri = new Uri(command);

            HttpResponseMessage response = (HttpResponseMessage)httpClientWrapper.GetWithUrl(uri).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.Content.ReadAsStreamAsync().Result;
                return stream;
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
