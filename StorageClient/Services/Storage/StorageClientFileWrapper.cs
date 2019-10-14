using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public class StorageClientFileWrapper : IStorageClientWrapper
    {
        public string BaseAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(int instanceOwnerOd, Guid instanceGuid)
        {
            throw new NotImplementedException();
        }
    }
}
