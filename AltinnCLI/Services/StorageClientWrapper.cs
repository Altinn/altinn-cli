using AltinnCLI.Commands.Core;
using AltinnCLI.Models;
using AltinnCLI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnCLI.Services
{
    public class StorageClientWrapper : IStorageClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageClientWrapper" /> class.
        /// </summary>
        public StorageClientWrapper(ILogger logger)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        /// <summary>
        /// Get a document from Storage for owner,instance and specific document id
        /// </summary>
        /// <param name="instanceOwnerId">owner id</param>
        /// <param name="instanceGuid">id of the instance</param>
        /// <param name="dataId">id of the data element/file</param>
        /// <returns></returns>
        public Stream GetData(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            string cmd = $@"instances/{instanceOwnerId}/{instanceGuid}/data/{dataId}";

            HttpClientWrapper httpClientWrapper = new(_logger);

            Task<HttpResponseMessage> response = httpClientWrapper.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }

        /// <summary>
        /// Fetches a document from Storage based on the URL for the document found in instance data
        /// </summary>
        /// <param name="command">Get URL</param>
        /// <param name="contentType">content type which must match content type in response if defined</param>
        /// <returns></returns>
        public Stream GetData(string command, string contentType = null)
        {
            HttpClientWrapper httpClientWrapper = new(_logger);
            Uri uri = new(command);

            HttpResponseMessage response = httpClientWrapper.GetWithUrl(uri, contentType).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return string.IsNullOrEmpty(contentType) ?
                    response.Content.ReadAsStreamAsync().Result :
                    string.Equals(contentType, response.Content.Headers.ContentType.ToString(), StringComparison.OrdinalIgnoreCase) ?
                    response.Content.ReadAsStreamAsync().Result :
                    null;
            }

            return null;
        }

        public InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId = null, Guid? instanceGuid = null)
        {
            string cmd = "instances";

            if (instanceOwnerId != null)
            {
                cmd += $@"?instanceOwner.partyId={instanceOwnerId}";

                if (instanceGuid != null)
                {
                    cmd += $@"\{instanceGuid}";
                }
            }

            HttpClientWrapper client = new(_logger);
            HttpResponseMessage response = client.GetCommand(BaseAddress, cmd).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responsMessage = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);
            }

            return null;
        }

        public InstanceResponseMessage GetInstanceMetaData(List<IOption> urlParams = null)
        {
            string cmd = "instances";

            IOption org = urlParams.FirstOrDefault(x => string.Equals(x.Name, "org", StringComparison.OrdinalIgnoreCase) && x.IsAssigned);
            IOption appid = urlParams.FirstOrDefault(x => string.Equals(x.Name, "appid", StringComparison.OrdinalIgnoreCase) && x.IsAssigned);

            if (org != null)
            {
                cmd += $@"?org={org.Value}";
                urlParams.Remove(org);
            }
            else
            {
                cmd += $@"?appId={appid.Value}";
                urlParams.Remove(appid);
            }

            foreach (IOption param in urlParams)
            {
                if (param.IsAssigned == true && !string.IsNullOrEmpty(param.ApiName))
                {
                    cmd += $@"&{param.ApiName}={param.Value}";
                }
            }
            HttpClientWrapper client = new(_logger);
            HttpResponseMessage response = client.GetCommand(BaseAddress, cmd).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responsMessage = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            }

            return null;
        }

        public InstanceResponseMessage GetInstanceMetaData(Uri uri)
        {
            HttpClientWrapper client = new(_logger);
            Task<HttpResponseMessage> response = client.GetWithUrl(uri);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public Stream GetInstances(int instanceOwnerId, Guid instanceGuid)
        {
            string cmd = $@"instances/{instanceOwnerId}/{instanceGuid}";

            HttpClientWrapper client = new(_logger);
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }

        /// <summary>
        /// Uploads a data element to storage 
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public InstanceResponseMessage UploadDataElement(List<IOption> urlParams, Stream data, string fileName)
        {
            // assumes that the values are validated by caller
            IOption instanceOwnerId = urlParams.FirstOrDefault(x => string.Equals(x.Name, "ownerid", StringComparison.OrdinalIgnoreCase));
            IOption instanceGuid = urlParams.FirstOrDefault(x => string.Equals(x.Name, "instanceid", StringComparison.OrdinalIgnoreCase));
            IOption dataType = urlParams.FirstOrDefault(x => string.Equals(x.Name, "elementtype", StringComparison.OrdinalIgnoreCase));

            string cmd = $@"instances/{instanceOwnerId.Value}/{instanceGuid.Value}/data?{dataType.ApiName}={dataType.Value}";
            string contentType = "application/xml";

            HttpClientWrapper client = new(_logger);
            StreamContent content = new(data);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data; name=" + Path.GetFileNameWithoutExtension(fileName));
            content.Headers.ContentDisposition.FileName = Path.GetFileName(fileName);
            Task<HttpResponseMessage> response = client.PostCommand(BaseAddress, cmd, content);

            if (response.Result.IsSuccessStatusCode)
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
