using Altinn.Platform.Storage.Interface.Models;

using AltinnCLI.Clients;
using AltinnCLI.Commands.Core;
using AltinnCLI.Helpers;
using AltinnCLI.Models;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AltinnCLI.Commands.Application.SubCommandHandlers
{
    class GetInstancesSubCommandHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        /// <summary>
        /// Handles communication with the runtime API
        /// </summary>
        private readonly InstanceClient _client;

        /// <summary>
        /// Creates an instance of <see cref="CreateInstanceSubCommandHandler" /> class
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information</param>
        public GetInstancesSubCommandHandler(InstanceClient client, ILogger<GetInstancesSubCommandHandler> logger) : base(logger)
        {
            _client = client;
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
                InstanceResponseMessage responsMessage = null;

                responsMessage = _client.GetAllAppInstances(org, app).Result;

                List<Instance> instances = new();

                if (responsMessage != null)
                {
                    instances.AddRange(responsMessage.Instances);

                    while (responsMessage.Next != null)
                    {
                        responsMessage = _client.GetInstances(responsMessage.Next).Result;
                        instances.AddRange(responsMessage.Instances);
                    }
                }

                string fileName = $"{DateTime.UtcNow}.json";

                string fileFolder = $@"{org}\{app}";

                if (CliFileWrapper.SaveToFile(fileFolder, fileName, JsonSerializer.Serialize(instances, typeof(List<Instance>), Globals.JsonSerializerOptions)))
                {
                    _logger.LogInformation($"File:{fileName} saved at {fileFolder}");
                }
            }

            return true;
        }
    }
}
