using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<string> ConvertToken(string token, bool test=false)
        {
            string AuthAddress = ApplicationManager.ApplicationConfiguration.GetSection("AuthBaseAddress").Get<string>();
            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            string cmd = $@"exchange/maskinporten?test={test}";
            AuthenticationHeaderValue headers = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClientWrapper.GetCommand(AuthAddress, cmd, headers);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<string>();
            }
            else
            {
                return $@"Could not retrieve Altinn Token";
            }
        }
    }
}
