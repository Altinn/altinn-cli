using AltinnCLI.Commands.Core;
using AltinnCLI.Services;
using AltinnCLI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AltinnCLI.Commands.Application.SubCommandHandlers
{
    class GetInstancesSubCommandHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        /// <summary>
        /// Handles communication with the runtime API
        /// </summary>
        private IApplicationClientWrapper clientWrapper = null;

        /// <summary>
        /// Creates an instance of <see cref="CreateInstanceSubCommandHandler" /> class
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information</param>
        public GetInstancesSubCommandHandler(ILogger<GetInstancesSubCommandHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                clientWrapper = new ApplicationClientWrapper(_logger);
            }
            else
            {
                clientWrapper = new ApplicationFileClientWrapper(_logger);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return "GetInstances";
            }
        }

        /// <summary>
        /// Gets the name of the CommandProvider for the command
        /// </summary>
        public string CommandProvider
        {
            get
            {
                return "Application";
            }
        }

        /// <summary>
        /// Gets the description of the command. Will be used to generate help documentation
        /// </summary>
        public string Description
        {
            get
            {
                return "Gets the list of instances for an application";
            }
        }

        /// <summary>
        /// Gets the usage of the command. Will be used to generate help documentation
        /// </summary>
        public string Usage
        {
            get
            {
                return $"AltinnCLI > Application GetInstances app org\n\n" +
                       $"\tapp <-a> \tApplication short code \n" +
                       $"\torg <-o> \tApplication owner identification code \n";
            }
        }

        public string GetHelp()
        {
            return Name;
        }

        public bool Run()
        {
            if (IsValid)
            {               
                    string app = (string)GetOptionValue("app");
                    string org = (string)GetOptionValue("org");
        
                string response = clientWrapper.GetInstances(app, org);
                _logger.LogInformation(response);
                
            }

            return true;
        }
    }
}
