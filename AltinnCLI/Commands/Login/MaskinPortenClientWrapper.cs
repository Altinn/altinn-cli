using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Commands.Login
{
    public class MaskinportenClientWrapper : IMaskinPortenClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationClientWrapper" /> class.
        /// </summary>
        public MaskinportenClientWrapper(ILogger logger)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("MaskinportenBaseAddress").Get<string>();
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        public string PostToken(FormUrlEncodedContent bearer)
        {
            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            string cmd = "token";

            Task<HttpResponseMessage> response = httpClientWrapper.PostCommand(BaseAddress, cmd, bearer);

            if (response.Result.IsSuccessStatusCode)
            {
                return response.Result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return $@"Could not retrieve Token";
            }
            
        }
    }
}
