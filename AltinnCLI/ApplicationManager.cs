﻿using AltinnCLI.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

using AltinnCLI.Core;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using AltinnCLI.Core.Json;
using Microsoft.Extensions.Logging;

namespace AltinnCLI
{
    /// <summary>
    /// Finds which type of service and command that should be executed according to user input. 
    /// Scans the CommandProvider that contains executable services and command handlers
    /// </summary>
    public class ApplicationManager
    {

        public static IConfigurationRoot ApplicationConfiguration;
        public static IServiceProvider ServiceProvider;

        private static ILogger _logger;

        public ApplicationManager(IServiceProvider serviceProvider, IConfigurationRoot applicationConfiguration, ILogger logger = null)
        {
            ServiceProvider = serviceProvider;
            ApplicationConfiguration = applicationConfiguration;
            _logger = logger;
        }

        /// <summary>
        /// Execute the user command with parameters. Finds the correct service and command handler and starts the service.
        /// </summary>
        /// <param name="args">User input from command line arguments</param>
        public void Execute(string args)
        {
            if (!string.IsNullOrEmpty(args))
            {
                string[] input = args.ToLower().Split(" ");

                ICommand service = ServiceProvider.GetServices<ICommand>().Where(s => string.Equals(s.Name, input[0], StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (service != null)
                {
                    if (input.Length > 1)
                    {
                        ISubCommandHandler commandHandler = processArgs(input);
                        if (commandHandler != null)
                        {
                            service.Run(commandHandler);
                        }
                        else
                        {
                            service.Run(ParseArguments(input));
                        }
                    }
                    else
                    {
                        service.Run();
                    }

                }
                else
                {
                    ServiceProvider.GetServices<IHelp>().FirstOrDefault().GetHelp();
                }
            }
        }

        /// <summary>
        /// Parses input parameters and finds the correct service and commmand handler for executing the command.
        /// </summary>
        /// <param name="input">
        /// User input from command line arguments, split into list of parameters separated by space. 
        /// Commands are structured as follows: [command] [subcommand] [options]
        /// </param>
        /// <returns>A commandHandler that can execute the command given by user input</returns>
        private ISubCommandHandler processArgs(string[] input)
        {
            ISubCommandHandler commandHandler = ApplicationManager.ServiceProvider.GetServices<ISubCommandHandler>()
                .FirstOrDefault(s => string.Equals(s.Name, input[1], StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.CommandProvider, input[0], StringComparison.OrdinalIgnoreCase));

            if (commandHandler != null)
            {
                commandHandler.DictOptions = ParseArguments(input);
                OptionBuilder.Instance(_logger).AssignValueToCliOptions(commandHandler);
                return commandHandler;
            }
            else
            {
                ApplicationManager.ServiceProvider.GetServices<IHelp>().FirstOrDefault().GetHelp();
            }

            return null;
        }

        /// <summary>
        /// Build Dictionary for input parameters
        /// </summary>
        /// <param name="args">Input parameters split into list of parameter names and values.</param>
        /// <returns></returns>
        protected Dictionary<string, string> ParseArguments(string[] args)
        {
            Dictionary<string, string> commandKeysAndValues = new Dictionary<string, string>();

            foreach (string param in args)
            {
                string[] cmdPar = param.Split("=");

                if (cmdPar.Length == 2)
                {
                    commandKeysAndValues.Add(cmdPar[0], cmdPar[1]);
                }
                else
                {
                    commandKeysAndValues.Add(cmdPar[0], string.Empty);
                }
            }

            return commandKeysAndValues;
        }
    }
}
