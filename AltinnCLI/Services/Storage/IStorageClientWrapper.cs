using Altinn.Platform.Storage.Models;
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

        Stream GetDocument(string command, string contentType = null);

        Stream GetInstances(int instanceOwnerOd, Guid instanceGuid);

        string CreateApplication(string appId, string instanceOwnerId, StringContent content);

        string CreateApplication(string app, string org, string instanceOwnerId, HttpContent content);

        InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId = null, Guid? instanceGuid = null);

        InstanceResponseMessage GetInstanceMetaData(Dictionary<string, string> urlParams = null);

        InstanceResponseMessage GetInstanceMetaData(Uri uri);

        InstanceResponseMessage UploadDataElement(string instanceOwnerId, string instanceGuid, string elementType, Stream data, string fileName);
    }
}
