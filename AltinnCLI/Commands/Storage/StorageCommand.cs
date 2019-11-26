using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltinnCLI.Commands.Storage
{
    public class StorageCommand : ICommand, IHelp
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        private static ILogger _logger;

        public StorageCommand(ILogger<StorageCommand> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subCommandHandler"></param>
        public virtual void Run(ISubCommandHandler subCommandHandler)
        {
            if (subCommandHandler != null)
            {
                subCommandHandler.Run();
            }
            else
            {
                _logger.LogError($"Missing sub command, use help to find avilable sub commands.");
            }
        }

        public string Name
        {
            get
            {
                return "Storage";
            }
        }

        public string Description
        {
            get
            {
                return "\tCommands for interacting with the Storage";
            }
        }

        public string Usage
        {
            get
            {
                return "storage <operation> -<option>";
            }
        }

        public string GetHelp()
        {
            return "Storage\nusage: storage <operation> -<option>\n\noperations:\ngetAttachment";
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }
    }
}
