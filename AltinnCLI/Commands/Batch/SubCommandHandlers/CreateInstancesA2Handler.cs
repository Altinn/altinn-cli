using AltinnCLI.Commands.Core;
using AltinnCLI.Services.Interfaces;

using Microsoft.Extensions.Logging;

using System;

namespace AltinnCLI.Commands.Batch.SubCommandHandlers
{
    public class CreateInstancesA2Handler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        private readonly IInstantiation _instantiationService;

        public CreateInstancesA2Handler(IInstantiation service, ILogger<CreateInstancesA2Handler> logger) : base(logger)
        {
            _instantiationService = service;
        }

        /// <summary>
        /// Gets the name of the command
        /// </summary>
        public string Name
        {
            get
            {
                return "CreateInstancesA2";
            }
        }

        /// <summary>
        /// Gets the description of the command. Will be used to generate help documentation
        /// </summary>
        public string Description
        {
            get
            {
                return "Create Altinn 3 instances from Altinn 2 prefill-data";
            }
        }

        /// <summary>
        /// Gets the usage of the command. Will be used to generate help documentation
        /// </summary>
        public string Usage
        {
            get
            {
                return $"AltinnCLI > Batch CreateInstancesA2 \n\n";
            }
        }

        /// <summary>
        /// Gets the name of the CommandProvider for the command
        /// </summary>
        public string CommandProvider
        {
            get
            {
                return "Batch";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes and runs the command
        /// </summary>
        /// <returns>Returns true if the command completes succesfully</returns>
        public bool Run()
        {
           return _instantiationService.Altinn2BatchInstantiation();
            
        }
    }
}
