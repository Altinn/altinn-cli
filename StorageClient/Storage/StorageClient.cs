using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altinn.Clients.StorageClient
{
    public class StorageClient
    {
        IStorageClientWrapper ClientWrapper;

        public MemoryStream GetDocument()
        {
            StorageClientWrapper wrapper = new StorageClientWrapper();

            return new MemoryStream();
        }
    }
}
