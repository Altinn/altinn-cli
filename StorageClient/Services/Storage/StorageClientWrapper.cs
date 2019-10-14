using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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

        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            
            string cmd = string.Format("{0}/{1}/data/{2}", instanceOwnerId, instanceGuid, dataId);

            HttpClientWrapper httpClinetWrapper = new HttpClientWrapper();

            Task<HttpResponseMessage> response =  httpClinetWrapper.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }

        public Stream GetInstances(int instanceOwnerId, Guid instanceGuid)
        {
            string cmd = string.Empty;
            cmd = string.Format("instances");

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }
    }
}
