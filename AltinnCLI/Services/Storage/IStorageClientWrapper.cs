using AltinnCLI.Core;
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

        Stream GetDocument(string command);

        Stream GetInstances(int instanceOwnerOd, Guid instanceGuid);

        string CreateApplication(string appId, string instanceOwnerId, StringContent content);

        string CreateApplication(string appId, string instanceOwnerId, HttpContent content);

        InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId = null, Guid? instanceGuid = null);

        InstanceResponseMessage GetInstanceMetaData(Uri uri);
    }
}
