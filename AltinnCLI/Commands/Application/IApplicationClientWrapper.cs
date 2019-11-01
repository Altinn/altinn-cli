using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Application
{
    public interface IApplicationClientWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="app"></param>
        /// <param name="instanceOwnerId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        string CreateApplication(string org, string app, string instanceOwnerId, HttpContent content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="instanceOwnerId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        string CreateApplication(string appId, string instanceOwnerId, StringContent content);
    }
}
