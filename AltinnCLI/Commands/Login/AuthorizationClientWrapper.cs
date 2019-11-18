using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Commands.Login
{
    class AuthorizationClientWrapper : IAutorizationClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationClientWrapper" /> class.
        /// </summary>
        public AuthorizationClientWrapper(ILogger logger)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("MaskinportenBaseAddress").Get<string>();
            _logger = logger;
        }

        public string ConvertToken(string token)
        {
            string AuthAddress = ApplicationManager.ApplicationConfiguration.GetSection("AuthBaseAddress").Get<string>();
            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            string cmd = "convert";
            AuthenticationHeaderValue headers = new AuthenticationHeaderValue("Bearer", token);
            Task<HttpResponseMessage> response = httpClientWrapper.GetCommand(AuthAddress, cmd, headers);

            if (response.Result.IsSuccessStatusCode)
            {
                return response.Result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return $@"Could not retrieve Altinn Token";
            }
        }
    }
}
