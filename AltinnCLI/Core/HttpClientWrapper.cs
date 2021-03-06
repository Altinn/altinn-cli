﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<HttpResponseMessage> GetCommand(string baseAddress, string command, AuthenticationHeaderValue headers = null)
        {
            Uri uri = new Uri(baseAddress + "/" + command);

            return await GetWithUrl(uri, null, headers);
        }

        /// <summary>
        /// Performs a request based on the Uri received as input. The content type is set if 
        /// supported in the content type parameter
        /// </summary>
        /// <param name="uri">Uri with URL that is the request url to be used</param>
        /// <param name="contentType">Default paramter that shall be set if not null</param>
        /// <returns>respons data</returns>
        public async Task<HttpResponseMessage> GetWithUrl(Uri uri, string contentType = null, AuthenticationHeaderValue headers = null)
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

                    if (headers != null )
                    {
                        client.DefaultRequestHeaders.Authorization = headers;
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationManager.MaskinportenToken);
                    }
                    
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
                using (HttpClient client = new HttpClient())
                {

                    client.Timeout = new TimeSpan(0, 0, 30);
                    if (!string.IsNullOrEmpty(ApplicationManager.MaskinportenToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationManager.MaskinportenToken);
                        // client.DefaultRequestHeaders.Add("Authorization", ApplicationManager.MaskinportenToken);
                    }

                    response = await client.PostAsync(uri, content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from ALtinn on command:\n {uri}. \nError: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n Exception: {ex} \n");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error respons from Altinn.error: HTTP {response.StatusCode}. Reason: {response.ReasonPhrase} \n");
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
