using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altinn.Clients.StorageClient
{
    public class StorageClient
    {

        public MemoryStream GetDocument()
        {
            StorageClientWrapper wrapper = new StorageClientWrapper();

            return new MemoryStream();
        }
    }
}
