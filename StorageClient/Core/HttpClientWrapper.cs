using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StorageClient
{
    public class HttpClientWrapper
    {
        /// <summary>
        /// Prepares requests and send it to FReg
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <returns>respons data</returns>
        public async Task<string> GetCommand(string baseAddress, string command)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            HttpResponseMessage response;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage message = new HttpRequestMessage();
                    message.Headers.Add("Accept", "application/json");
                    message.Headers.Add("ContentType", "application/json");
 //                   client.Timeout = new TimeSpan(0, 0, 0, 10, AltinnConfiguration.FRegEndpointTimeoutInMS);
                    response = await client.GetAsync(uri).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("FReg Connection error:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                string errorMessage = string.Format("Error getting data from FREg on command: {0}. Error: HTTP {1}. Reason: {2}", uri, response.StatusCode, response.ReasonPhrase);
                throw new System.Exception(errorMessage);
            }

            string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Prepares requests and send it to FReg
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <param name="content">The content of the post message</param>
        /// <returns>respons data</returns>
        public async Task<string> PostCommand(string baseAddress, string command, StringContent content)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            HttpResponseMessage response;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //client.Timeout = new TimeSpan(0, 0, 0, 10, AltinnConfiguration.FRegEndpointTimeoutInMS);

                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("FReg Connection error:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                string errorMessage = string.Format("Error getting event feed from FREg. error: HTTP {0}. Reason: {1}", response.StatusCode, response.ReasonPhrase);
                throw new System.Exception(errorMessage);
            }

            string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return result;
        }
    }
}
