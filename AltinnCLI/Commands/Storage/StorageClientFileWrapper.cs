using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AltinnCLI.Core;
using Microsoft.Extensions.Logging;

namespace AltinnCLI.Commands.Storage
{
    /// <summary>
    /// This is a class that shall be used in unit test where access to external API's 
    /// is not available. 
    /// </summary>
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

        public string BaseAddress { get; set; }

        public static InstanceResponseMessage? InstanceResponse { get; set; }

        public static MemoryStream DataContent { get; set; }

        public static bool IsSuccessStatusCode;

        public Stream GetData(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            throw new NotImplementedException();
        }

        public Stream GetData(string command, string contentType = null)
        {
            return DataContent;
        }

        public InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId = null, Guid? instanceGuid = null)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage GetInstanceMetaData(Uri uri)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage GetInstanceMetaData(List<IOption> urlParams = null)
        {
            return InstanceResponse;
        }

        public Stream GetInstances(int instanceOwnerOd, Guid instanceGuid)
        {
            throw new NotImplementedException();
        }

        public InstanceResponseMessage UploadDataElement(List<IOption> urlParams, Stream data, string fileName)
        {

            if (IsSuccessStatusCode)
            {
                _logger.LogInformation($"The file: {fileName} was successfully uploaded");
            }
            else
            {
                _logger.LogError($"Failed to upload the file: {fileName}");
            }

            return null;
        }
    }
}
