using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using AltinnCLI.Core;
using Microsoft.Extensions.Logging;

namespace AltinnCLI.Commands.Storage
{
    public class StorageClientFileWrapper : IStorageClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        public StorageClientFileWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public string BaseAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Stream GetData(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            throw new NotImplementedException();
        }

        public Stream GetData(string command, string contentType = null)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId = null, Guid? instanceGuid = null)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage GetInstanceMetaData(Uri uri)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage GetInstanceMetaData(Dictionary<string, string> urlParams = null)
        {
            throw new NotImplementedException();
        }

        public Stream GetInstances(int instanceOwnerOd, Guid instanceGuid)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage UploadDataElement(string instanceOwnerId, string instanceGuid, string elementType, Stream data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
