using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public interface IStorageClientWrapper
    {
        Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId);
    }
}
