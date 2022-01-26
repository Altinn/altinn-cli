using AltinnCLI.Commands.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Commands.Application
{
    class ApplicationCommand : ICommand, IHelp
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        private static ILogger _logger;

        public ApplicationCommand(ILogger<ApplicationCommand> logger)
        {
            _logger = logger;
        }

        public string Name
        {
            get
            {
                return "Application";
            }
        }

        public string Description
        {
            get
            {
                return "Tools for creating an application";
            }
        }

        public string Usage
        {
            get
            {
                return "DO NOT USE!";
            }
        }

        public string GetHelp()
        {
            return "";
        }

        public virtual void Run(ISubCommandHandler subCommandHandler)
        {
            if (subCommandHandler != null)
            {
                subCommandHandler.Run();
            }
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }
    }
}
