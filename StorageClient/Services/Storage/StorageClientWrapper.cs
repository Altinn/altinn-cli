using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Services.Storage
{
    public class StorageClientWrapper : IStorageClientWrapper
    {
        public StorageClientWrapper()
        {
        }

        public string BaseAddress { get; set; }

        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            String cmd = string.Empty;
            string baseAddress = System.Configuration.ConfigurationManager.AppSettings.Get("BaseAddress");

            HttpClientWrapper httpClinetWrapper = new HttpClientWrapper();

            //return httpClinetWrapper.GetCommand(baseAddress, cmd);

            return new MemoryStream();
        }
    }
}
