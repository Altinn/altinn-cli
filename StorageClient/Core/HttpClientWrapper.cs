using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Core
{
    public class HttpClientWrapper
    {
        /// <summary>
        /// Prepares requests and send it to FReg
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> GetCommand(string baseAddress, string command)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            return await GetWithUrl(uri);
        }

        public async Task<HttpResponseMessage> GetWithUrl(Uri uri)
        {
            HttpResponseMessage response;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage message = new HttpRequestMessage();
                    message.Headers.Add("Accept", "application/json");
                    message.Headers.Add("ContentType", "application/json");
                    response = await client.GetAsync(uri).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                string errorMessage = $"Error getting data from ALtinn on command: {uri}. Error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}";
                throw new System.Exception(errorMessage);
            }

            return response;
        }

        /// <summary>
        /// Prepares requests and send it to FReg
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
                string errorMessage = $"Error respons from ALtinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}";
                throw new System.Exception(errorMessage);
            }

            return response;
        }
    }
}
