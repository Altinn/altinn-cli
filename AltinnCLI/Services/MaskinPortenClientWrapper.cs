using AltinnCLI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace AltinnCLI.Services
{
    public class MaskinportenClientWrapper : IMaskinPortenClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
       private readonly ILogger _logger;

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

        public bool PostToken(FormUrlEncodedContent bearer, out string token)
        {
            HttpClientWrapper httpClientWrapper = new(_logger);
            token = string.Empty;
            string cmd = "token";

            Task<HttpResponseMessage> response = httpClientWrapper.PostCommand(BaseAddress, cmd, bearer);

            if (response.Result.IsSuccessStatusCode)
            {
                token = response.Result.Content.ReadAsStringAsync().Result;
                return true;
            }
            else
            {
                _logger.LogError(@"Could not retrieve Token");
            }

            return false;

        }
    }
}
