using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public interface IStorageClientWrapper
    {
        Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId);

        Stream GetInstances(int instanceOwnerOd, Guid instanceGuid);

        string CreateApplication(string appId, string instanceOwnerId, StringContent content);
    }
}
