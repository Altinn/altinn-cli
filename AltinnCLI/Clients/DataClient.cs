using AltinnCLI.Commands.Core;
using AltinnCLI.Models;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnCLI.Clients
{
    public class DataClient
    {
        private readonly HttpClient _client;

        public DataClient(HttpClient client)
        {

            _client = client;
        }

        public async Task<Stream> GetData(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            requestUri += $@"/instances/{instanceOwnerId}/{instanceGuid}/data/{dataId}";
            HttpResponseMessage response =await  _client.GetAsync(requestUri);

            Stream stream = await response.Content.ReadAsStreamAsync();

            return stream;

        }

        public async Task<Stream> GetData(string selfLink, string contentType = null)
        {
            HttpRequestMessage msg = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(selfLink)
            };

            msg.Headers.Add("ContentType", contentType);

            HttpResponseMessage response = await _client.SendAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Could not upload data element for instance {instanceOwnerId}/{instanceGuid}. Error code: { response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            if (string.IsNullOrEmpty(contentType))
            {
                return response.Content.ReadAsStreamAsync().Result;
            }

            return string.Equals(contentType, response.Content.Headers.ContentType.ToString(), StringComparison.OrdinalIgnoreCase) ?
                response.Content.ReadAsStreamAsync().Result :
                null;
        }

        public async Task<InstanceResponseMessage> UploadDataElement(List<IOption> urlParams, Stream data, string fileName)
        {
            // assumes that the values are validated by caller
            IOption instanceOwnerId = urlParams.FirstOrDefault(x => string.Equals(x.Name, "ownerid", StringComparison.OrdinalIgnoreCase));
            IOption instanceGuid = urlParams.FirstOrDefault(x => string.Equals(x.Name, "instanceid", StringComparison.OrdinalIgnoreCase));
            IOption dataType = urlParams.FirstOrDefault(x => string.Equals(x.Name, "elementtype", StringComparison.OrdinalIgnoreCase));

            string requestUri = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            requestUri += $@"instances/{instanceOwnerId.Value}/{instanceGuid.Value}/data?{dataType.ApiName}={dataType.Value}";
            string contentType = "application/xml";


            StreamContent content = new(data);

            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data; name=" + Path.GetFileNameWithoutExtension(fileName));
            content.Headers.ContentDisposition.FileName = Path.GetFileName(fileName);
            HttpResponseMessage response = await _client.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Could not upload data element for instance {instanceOwnerId}/{instanceGuid}. Error code: { response.StatusCode} Error message: {response.ReasonPhrase}");
            }

            return null;
        }

    }
}
