using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Services.Storage
{
    public class StorageClientWrapper : IStorageClientWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageClientWrapper" /> class.
        /// </summary>
        public StorageClientWrapper()
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        public string CreateApplication(string appId, string instanceOwnerId, HttpContent content)
        {
            string cmd = $@"instances?appId={appId}&instanceOwnerId={instanceOwnerId}";

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper();

            Task<HttpResponseMessage> response = httpClientWrapper.PostCommand(BaseAddress, cmd, content);

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

            HttpClientWrapper httpClinetWrapper = new HttpClientWrapper();

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

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper();
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

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public InstanceResponseMessage GetInstanceMetaData(Uri uri)
        {

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetWithUrl(uri);

            string responsMessage = response.Result.Content.ReadAsStringAsync().Result;

            InstanceResponseMessage instanceMessage = JsonConvert.DeserializeObject<InstanceResponseMessage>(responsMessage);

            return instanceMessage;
        }

        public Stream GetInstances(int instanceOwnerId, Guid instanceGuid)
        {
            string cmd = "instances";

            HttpClientWrapper client = new HttpClientWrapper();
            Task<HttpResponseMessage> response = client.GetCommand(BaseAddress, cmd);

            Stream stream = response.Result.Content.ReadAsStreamAsync().Result;

            return stream;
        }


    }
}
