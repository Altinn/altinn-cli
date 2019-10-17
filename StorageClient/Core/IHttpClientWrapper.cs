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
            /// Prepares and requests FReg
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <returns>respons datat</returns>
            Task<HttpResponseMessage> GetCommand(string baseAddress, string command);


            Task<HttpResponseMessage> GetWithUrl(string command);

            /// <summary>
            /// Prepares and requests FReg
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <param name="content">The content of the post message</param>
            /// <returns>respons datat</returns>
            Task<HttpResponseMessage> PostCommand(string baseAddress, string command, StringContent content);
        }
    }
}
