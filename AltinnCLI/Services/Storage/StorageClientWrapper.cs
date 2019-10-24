using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Services.Storage
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

        public string CreateApplication(string org, string app, string instanceOwnerId, HttpContent content)
        {
            string AppBaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("AppAPIBaseAddress").Get<string>().Replace("{org}", org);
            string cmd = $@"{org}/{app}/instances";
            
            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            Task <HttpResponseMessage> response = httpClientWrapper.PostCommand(AppBaseAddress, cmd, content);

            return response.Result.Content.ReadAsStringAsync().Result;

        }

        public string CreateApplication(string appId, string instanceOwnerId, StringContent content)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a document from Storage for owner,instance and specific document id
        /// </summary>
        /// <param name="instanceOwnerId">owner id</param>
        /// <param name="instanceGuid">id of the instance</param>
        /// <param name="dataId">id of the data element/file</param>
        /// <returns></returns>
        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            string cmd = $@"instances/{instanceOwnerId}/{instanceGuid}/data/{dataId}";

            HttpClientWrapper httpClinetWrapper = new HttpClientWrapper(_logger);

            Task<HttpResponseMessage> response = httpClinetWrapper.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }

        /// <summary>
        /// Fetches a document from Storage based on the URL for the document found in instance data
        /// </summary>
        /// <param name="command">Get URL</param>
        /// <param name="contentType">content type which must match content type in repons if defined</param>
        /// <returns></returns>
        public Stream GetDocument(string command, string contentType = null)
        {

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);
            Uri uri = new Uri(command);

            HttpResponseMessage response = (HttpResponseMessage)httpClientWrapper.GetWithUrl(uri, contentType).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return (string.IsNullOrEmpty(contentType) ?
                    response.Content.ReadAsStreamAsync().Result :
                    (string.Equals(contentType, response.Content.Headers.ContentType.ToString(), StringComparison.OrdinalIgnoreCase)) ?
                    response.Content.ReadAsStreamAsync().Result :
                    null);
            }

            return null;
        }

        public InstanceResponseMessage GetInstanceMetaData(int? instanceOwnerId=null, Guid? instanceGuid = null)
        {
            string cmd = "instances";

            if (instanceOwnerId != null)
            {
                cmd += $@"?instanceOwnerId={instanceOwnerId}";

                if (instanceGuid != null)
                {
                    cmd += $@"\{instanceGuid}";
                }
            }

            HttpClientWrapper client = new HttpClientWrapper(_logger);
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public InstanceResponseMessage GetInstanceMetaData(string appId, Dictionary<string,string> urlParams = null )
        {
            string cmd = "instances";

            if (urlParams != null)
            {
                if (urlParams.ContainsKey("org"))
                {
                    cmd += $@"?org={urlParams["org"]}";
                }
                else if (urlParams.ContainsKey("appid"))
                {
                    cmd += $@"?appId={urlParams["appid"]}";
                }

                if (urlParams.ContainsKey("currenttaskid"))
                {
                    cmd += $@"&process.currentTask={urlParams["currenttaskid"]}";
                }

                if (urlParams.ContainsKey("processiscomplete"))
                {
                    cmd += $@"&process.isComplete={urlParams["processiscomplete"]}";
                }

                if (urlParams.ContainsKey("processisinerror"))
                {
                    cmd += $@"&process.isInError={urlParams["processisinerror"]}";
                }

                if (urlParams.ContainsKey("processendstate"))
                {
                    cmd += $@"&process.endState={urlParams["processendstate"]}";
                }

                if (urlParams.ContainsKey("lastchangeddatetime"))
                {
                    cmd += $@"&lastChangedDateTime={urlParams["lastchangeddatetime"]}";
                }

                if (urlParams.ContainsKey("createddatetime"))
                {
                    cmd += $@"&createdDateTime={urlParams["createddatetime"]}";
                }

                if (urlParams.ContainsKey("visibledatetime"))
                {
                    cmd += $@"&visibleDateTime={urlParams["visibledatetime"]}";
                }

                if (urlParams.ContainsKey("duedatetime"))
                {
                    cmd += $@"&dueDateTime={urlParams["duedatetime"]}";
                }

                if (urlParams.ContainsKey("continuationToken"))
                {
                    cmd += $@"&continuationToken={urlParams["continuationToken"]}";
                }

                if (urlParams.ContainsKey("size"))
                {
                    cmd += $@"&size={urlParams["size"]}";
                }
            }

            HttpClientWrapper client = new HttpClientWrapper(_logger);
            HttpResponseMessage response = (HttpResponseMessage)client.GetCommand(BaseAddress, cmd).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responsMessage = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            }

            return null;
        }

        public InstanceResponseMessage GetInstanceMetaData(Uri uri)
        {

            HttpClientWrapper client = new HttpClientWrapper(_logger);
            Task<HttpResponseMessage> response = client.GetWithUrl(uri);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public Stream GetInstances(int instanceOwnerId, Guid instanceGuid)
        {
            string cmd = "instances";

            HttpClientWrapper client = new HttpClientWrapper(_logger);
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }



        /// <summary>
        /// /instances/{instanceOwnerId}/{instanceGuid}/data?elementType={elementType}
        /// </summary>
        /// <param name="instanceOwnerId">owner id</param>
        /// <param name="instanceGuid">id of the instance</param>
        /// <param name="elementType">type of document to upload</param>
        public InstanceResponseMessage UploadDataElement(string instanceOwnerId, string instanceGuid, string elementType, Stream data, string fileName)
        {
            string cmd = $@"instances/{instanceOwnerId}/{instanceGuid}/data?elementType={elementType}";
            string contentType = "application/xml";

            HttpClientWrapper client = new HttpClientWrapper(_logger);
            StreamContent content = new StreamContent(data);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data; name=" + Path.GetFileNameWithoutExtension(fileName));
            content.Headers.ContentDisposition.FileName = Path.GetFileName(fileName);
            Task<HttpResponseMessage> response = client.PostCommand(BaseAddress, cmd, content);

            return null;
        }


    }
}
