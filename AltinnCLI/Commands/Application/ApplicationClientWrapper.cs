using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Commands.Application
{
    class ApplicationClientWrapper : IApplicationClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationClientWrapper" /> class.
        /// </summary>
        public ApplicationClientWrapper(ILogger logger)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="app"></param>
        /// <param name="instanceOwnerId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string CreateInstance(string org, string app, string instanceOwnerId, HttpContent content)
        {
            string AppBaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("AppAPIBaseAddress").Get<string>().Replace("{org}", org);
            string cmd = $@"{org}/{app}/instances";

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            Task<HttpResponseMessage> response = httpClientWrapper.PostCommand(AppBaseAddress, cmd, content);


            if (response.Result.IsSuccessStatusCode)
            {
                return response.Result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return $@"Could not create application instance. Error code: {response.Result.StatusCode} Error message: {response.Result.ReasonPhrase}";
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="instanceOwnerId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string CreateInstance(string appId, string instanceOwnerId, StringContent content)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        public string GetInstances(string app, string org)
        {
            string BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            string cmd = $@"instances?&appId={org}/{app}";

            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            Task<HttpResponseMessage> response = httpClientWrapper.GetCommand(BaseAddress, cmd);


            if (response.Result.IsSuccessStatusCode)
            {
                return response.Result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return $@"Could not create application instance. Error code: {response.Result.StatusCode} Error message: {response.Result.ReasonPhrase}";
            }
        }
    }
}
