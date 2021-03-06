﻿using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.IO;

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

        InstanceResponseMessage UploadDataElement(List<IOption> urlParams, Stream data, string fileName);
    }
}
