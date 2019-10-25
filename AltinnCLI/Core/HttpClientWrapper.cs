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

        public HttpClientWrapper(ILogger logger)
        {
            _logger = logger;
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
               _logger.LogError($"Error getting data from ALtinn on command:\n {uri}. \nError: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
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
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
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

            HttpResponseMessage response = null;

            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer} )
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 30);

                    /*
                    cookieContainer.Add(uri, new Cookie("AltinnStudioRuntime", "eyJhbGciOiJSUzI1NiIsImtpZCI6IjAwQTEyRDkyRTRDNEMxQTI5REZCOTU2QTAzMzQwNDYwRDYwNTlDMDkiLCJ4NXQiOiJBS0V0a3VURXdhS2QtNVZxQXpRRVlOWUZuQWsiLCJ0eXAiOiJKV1QifQ.eyJVc2VySUQiOiIyMDAwMzUyMCIsIlVzZXJOYW1lIjoiIiwiUGFydHlJRCI6NTAwMTI5NjAsIkF1dGhlbnRpY2F0ZU1ldGhvZCI6IkFsdGlublBJTiIsIkF1dGhlbnRpY2F0aW9uTGV2ZWwiOjIsIm5iZiI6MTU3MTc1NTA0OSwiZXhwIjoxNTcxNzU2ODQ5LCJpYXQiOjE1NzE3NTUwNDl9.whiKfHP-LwgX6t2yh96mjSxu3XejYp3op0Q-fzQ_wfDqSS8ouVlSZ5XOb3TZOiblKRAnf0UCslmSGl3NyznJHVgMOL5pfZ8rCvubFkpADeGH9zsaty8reOlKF0vWTXFo54MgU5v3Kmctf6L7yMpp78URcm8J6bG0ciJq8S1pDOCv2yowC6hLzalgswaU_t4LsRyd1msWgYZrg6KhrrPIUMgW8-QpAZz4AVnGEFkIZgML0z2Z9C9vtACw205pBfe6y52h6kbMRRebFTS8fRG0HBZjhiT_SPZM1iRb75DpyuAHREGdh0LCjJ5-OLVfJVxR0qIG7TsFId51auvHjlg7nA"));
                    cookieContainer.Add(uri, new Cookie("AltinnPartyId", "50012960"));
                    cookieContainer.Add(uri, new Cookie(".ASPXAUTH", "AC624169D5FC4AF960FF60F22E36DACE912F98F3989858BAB85C8D483466F358373FFAEDB6C5009034EE4F09B8D83202AFED6AF58C34D42B21732DA6A0C89576CA077835A7B4F9C78C0F8FC162A84FB97C4393563D6BB0B16053DBF53B11727ABC6914501CAC741DD38C0F8F79E17D67117A6295C21A3D997CFDEE32ACFA3C8B73931B847D490873016EF7591E2AFB31C0867F90223839B400C75420C740F0882F25946C5A6E0BC0705EDF8766AC592DBA1A5649F55F2B3F042B49E7E14A0C2BB185EEC959557CA64D93D4C40408E98E9E61B2455FE5B7944FAA5957AA0467D7E1D6E71DC62F374B10DE08FEC63AC6017CA9CDB7F24DAA102B52C4030948117D903FB6634BA242C6A3478EFCD62659B8F5933C8A"));
                    */

                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from ALtinn on command:\n {uri}. \nError: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
            }

            return response;
        }

        /// <summary>
        /// Prepares requests and sends it as a PUT request
        /// </summary>
        /// <param name="baseAddress">Base Address for data source</param>
        /// <param name="command">The http command</param>
        /// <param name="content">The content body for the PUT message</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> PutCommand(string baseAddress, string command, HttpContent content)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            HttpResponseMessage response;

            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (HttpClient client = new HttpClient(handler))
                {
                    response = await client.PutAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase}");
            }

            return response;
        }
    }
}
