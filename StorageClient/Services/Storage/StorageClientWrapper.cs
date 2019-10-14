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
            String cmd = string.Empty;

            HttpClientWrapper httpClinetWrapper = new HttpClientWrapper();

            //return httpClinetWrapper.GetCommand(BaseAddress, cmd);

            return new MemoryStream();
        }

        public HttpResponseMessage GetInstances(int instanceOwnerId, Guid instanceGuid)
        {
            string cmd = string.Empty;
            cmd = string.Format("instances");

            HttpClientWrapper client = new HttpClientWrapper();
            return client.GetCommand(BaseAddress, cmd).Result;
        }
    }
}
