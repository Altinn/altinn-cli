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

                    client.DefaultRequestHeaders.Add("Authorization", ApplicationManager.MaskinportenToken);
                    message.RequestUri = uri;
                    _logger.LogInformation($"Get data with URL:{uri}\n");
                    response = await client.SendAsync(message).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
               _logger.LogError($"Error getting data from ALtinn on command:\n {uri}. \nError: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n");
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
                    client.DefaultRequestHeaders.Add("Authorization", ApplicationManager.MaskinportenToken);
                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n Url:{uri} \n");
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

                    
                    cookieContainer.Add(uri, new Cookie("AltinnStudioRuntime", "eyJhbGciOiJSUzI1NiIsImtpZCI6IjAwQTEyRDkyRTRDNEMxQTI5REZCOTU2QTAzMzQwNDYwRDYwNTlDMDkiLCJ4NXQiOiJBS0V0a3VURXdhS2QtNVZxQXpRRVlOWUZuQWsiLCJ0eXAiOiJKV1QifQ.eyJVc2VySUQiOiIyMDAwMjk0MyIsIlVzZXJOYW1lIjoiYXQyMXRlc3R1c2VyIiwiUGFydHlJRCI6NTAwMTIzNzMsIkF1dGhlbnRpY2F0ZU1ldGhvZCI6IlN0YXRpY1Bhc3N3b3JkIiwiQXV0aGVudGljYXRpb25MZXZlbCI6MSwibmJmIjoxNTcyOTUxMTk2LCJleHAiOjE1NzI5NTI5OTYsImlhdCI6MTU3Mjk1MTE5Nn0.OsJJM8jPEHE6yf5HhfaFgxL-WeWOQXt0_U8ApGWZUR6BDXPzfjVdKlvsEwT33X4G6BYJucZ3YaASuhn8fZkpx47raXtY7-lLtqnLpB-yOTtPoWqDEKJ0noLvhkVEaO1ZC0RezYVPOXu2iCXZerJlgXAlj52mQac0GW2VQ6QrSAOWa11H7Yui4Ee1HWVzmWmUawUeTSaHA29wfubYLuk1yo5BwloXdfOWEiUbxUg9doYbmY8pXoxyqFor04bP2Oa-yPAm8C_3vM4ab8nKUvhls-xVDbyTS-zdUOiYQV8INxzkcH-0oiMu7DEn7lUWTEW9tGm8GPJpdnoh4t0J5hcfbA; path=/; domain=.at21.altinn.cloud; HttpOnly;"));
                    // cookieContainer.Add(uri, new Cookie("AltinnPartyId", "50012960"));
                    cookieContainer.Add(uri, new Cookie(".ASPXAUTH", "FC148DD7A5CB06859FF1190D023F9CCA424A03637F53033B7837BF6F56E6CDE9A9945C3956BFDFA7F993CF116606FEE5600195DE67BBD5075F9C4BC9892B39BAB64D874E1A5C9FF0CF27351117B21A406D2E6E9141104831467B3C35C8AAED90C5F83B1EB7B9D4AF9EDAF656F4B8CAF85E5AFABBB99B083D5A2D410679B90D2C6558573CF2651B0CA66EA9D0805E95DF37EE7B7BFC733B9C0CAFD530E04A865BF56403F66B855804D3A451160E266222512BB0E170553FD7ADF6A34E41C32454E29DCDB778A8B494E99B10B1D6446202C6B3559BC4542A57374A0906BB63139CACCE88958222BFDE15FA1C82619058BD1FA8572F21408CD4456F89A58ED9156932079D4516908E68B3980B98E795DA77A67EC5AA9D49611BAADD349C68175D9AFBE996B5; path=/; domain=.at21.altinn.cloud; HttpOnly;"));

                    client.DefaultRequestHeaders.Add("Authorization", ApplicationManager.MaskinportenToken);
                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from ALtinn on command:\n {uri}. \nError: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n Exception: {ex} \n");
            }

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n");
            }
            else
            {
                _logger.LogInformation($"Successfully uploaded file. Url:{uri} \n");
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
                    client.DefaultRequestHeaders.Add("Authorization", ApplicationManager.MaskinportenToken);
                    response = await client.PutAsync(uri, content).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to connect to Altinn:", ex);
            }

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n Url:{uri} \n");
            }

            return response;
        }
    }
}
