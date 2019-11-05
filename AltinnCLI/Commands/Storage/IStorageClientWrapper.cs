using Altinn.Platform.Storage.Models;
using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Storage
{
    public interface IStorageClientWrapper
    {
        Stream GetData(int instanceOwnerId, Guid instanceGuid, Guid dataId);

        Stream GetData(string command, string contentType = null);

        Stream GetInstances(int instanceOwnerOd, Guid instanceGuid);

        InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId = null, Guid? instanceGuid = null);

        InstanceResponseMessage GetInstanceMetaData(List<IOption> urlParams = null);

        InstanceResponseMessage GetInstanceMetaData(Uri uri);

        InstanceResponseMessage UploadDataElement(string instanceOwnerId, string instanceGuid, string elementType, Stream data, string fileName);

        InstanceResponseMessage UploadDataElement(List<IOption> urlParams, Stream data, string fileName);
    }
}
