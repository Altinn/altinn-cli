using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Contains the preparations and http requests
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly ILogger _logger;

        public HttpClientWrapper()
        {
        }

        /// <summary>
        /// Adds base address to tht request URL and call method to perform the request
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> GetCommand(string baseAddress, string command)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            return await GetWithUrl(uri);
        }

        /// <summary>
        /// Performs a request based on the Uri received as input. The content type is set if 
        /// supported in the content type parameter
        /// </summary>
        /// <param name="uri">Uri with URL that is the request url to be used</param>
        /// <param name="contentType">Default paramter that shall be set if not null</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> GetWithUrl(Uri uri, string contentType = null)
        {
            HttpResponseMessage response;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage message = new HttpRequestMessage();
                    message.Headers.Add("Accept", "application/json");
                    if (string.IsNullOrEmpty(contentType))
                    {
                        message.Headers.Add("ContentType", "application/json");
                    }
                    else
                    {
                        message.Headers.Add("ContentType", contentType);
                    }

                    message.RequestUri = uri;

                    response = await client.SendAsync(message).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
               _logger.LogError($"Error getting data from ALtinn on command: {uri}. Error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
            }

            return response;
        }

        /// <summary>
        /// Prepares requests and sends it
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <param name="content">The content of the post message</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> PostCommand(string baseAddress, string command, StringContent content)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            HttpResponseMessage response;
            
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error respons from ALtinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
            }

            return response;
        }

        /// <summary>
        /// Prepares requests and sends it
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <param name="content">The content of the post message</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> PostCommand(string baseAddress, string command, HttpContent content)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            HttpResponseMessage response;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error respons from ALtinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
            }

            return response;
        }
    }
}
