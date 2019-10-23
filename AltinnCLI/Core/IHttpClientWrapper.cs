using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Core
{
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// Contains the preparations and http request agains FReg
        /// </summary>
        public interface IHttpClientWrapper
        {
            /// <summary>
            /// Prepares and requests to baseAddress
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <returns>respons datat</returns>
            Task<HttpResponseMessage> GetCommand(string baseAddress, string command);


            /// <summary>
            /// Performs a request based on the Uri received as input. The content type is set if 
            /// supported in the content type parameter
            /// </summary>
            /// <param name="uri">Uri with URL that is the request url to be used</param>
            /// <param name="contentType">Default paramter that shall be set if not null</param>
            /// <returns>respons data</returns>
            Task<HttpResponseMessage> GetWithUrl(Uri uri, string contentType = null);

            /// <summary>
            /// Prepares requests and sends it
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <param name="content">The content of the post message</param>
            /// <returns>respons data</returns>
            Task<HttpResponseMessage> PostCommand(string baseAddress, string command, StringContent content);

            /// <summary>
            /// Prepares requests and sends it
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <param name="content">The content of the post message</param>
            /// <returns>respons data</returns>
            Task<HttpResponseMessage> PostCommand(string baseAddress, string command, HttpContent content);

            /// <summary>
            /// Prepares a PUT requests and sends it
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <param name="content">The content of the PUT message</param>
            /// <returns>respons data</returns>
            Task<HttpResponseMessage> PutCommand(string baseAddress, string command, HttpContent content);
        } 
    }
}
