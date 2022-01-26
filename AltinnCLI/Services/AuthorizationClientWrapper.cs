using AltinnCLI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AltinnCLI.Services
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
        /// Initializes a new instance of the <see cref="AuthorizationClientWrapper" /> class.
        /// </summary>
        public AuthorizationClientWrapper(ILogger logger)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("MaskinportenBaseAddress").Get<string>();
            _logger = logger;
        }

        public async Task<string> ConvertToken(string token, bool test = false)
        {
            string authAddress = ApplicationManager.ApplicationConfiguration.GetSection("AuthBaseAddress").Get<string>();
            HttpClientWrapper httpClientWrapper = new HttpClientWrapper(_logger);

            string cmd = $@"exchange/maskinporten?test={test}";
            AuthenticationHeaderValue headers = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClientWrapper.GetCommand(authAddress, cmd, headers);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<string>(jsonString);
            }
            else
            {
                return $@"Could not retrieve Altinn Token";
            }
        }
    }
}
