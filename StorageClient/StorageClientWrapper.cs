﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Altinn.Clients.StorageClient
{
    public class StorageClientWrapper : IStorageClientWrapper
    {
        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            String cmd = string.Empty;
            string baseAddress = System.Configuration.ConfigurationManager.AppSettings.Get("BaseAddress");

            HttpStorageClientWrapper httpClinetWrapper = new HttpStorageClientWrapper();

            //return httpClinetWrapper.GetCommand(baseAddress, cmd);

            return new MemoryStream();
        }
    }
}
