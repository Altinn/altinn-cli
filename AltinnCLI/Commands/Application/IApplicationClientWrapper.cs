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
        string CreateInstance(string org, string app, string instanceOwnerId, HttpContent content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="instanceOwnerId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        string CreateInstance(string appId, string instanceOwnerId, StringContent content);

        /// <summary>
        /// Gets a list of insances for an application
        /// </summary>
        /// <param name="app">name of the application</param>
        /// <param name="org">Name of the application owner</param>
        /// <returns></returns>
        string GetInstances(string app, string org);
    }
}
