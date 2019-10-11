using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageClient
{
    public interface IStorageClientWrapper
    {
        string BaseAddress { get; set; }

        Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId);
    }
}
