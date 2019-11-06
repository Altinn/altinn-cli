using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Application
{
    class ApplicationFileClientWrapper : IApplicationClientWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFileClientWrapper" /> class.
        /// </summary>
        public ApplicationFileClientWrapper(ILogger logger)
        {
            BaseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        public string CreateInstance(string org, string app, string instanceOwnerId, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public string CreateInstance(string appId, string instanceOwnerId, StringContent content)
        {
            throw new NotImplementedException();
        }
    }
}
