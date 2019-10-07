using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageClient
{
    public interface IHttpStorageClientWarapper
    {
        /// <summary>
        /// Contains the preparations and http request agains FReg
        /// </summary>
        public interface IFRegHttpClientWrapper
        {
            /// <summary>
            /// Prepares and requests FReg
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <returns>respons datat</returns>
            Task<string> GetCommand(string baseAddress, string command);

            /// <summary>
            /// Prepares and requests FReg
            /// </summary>
            /// <param name="baseAddress">Base Address for data source</param>
            /// <param name="command">The http command</param>
            /// <param name="content">The content of the post message</param>
            /// <returns>respons datat</returns>
            Task<string> PostCommand(string baseAddress, string command, StringContent content);
        }
    }
}
