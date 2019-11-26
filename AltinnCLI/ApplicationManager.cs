using AltinnCLI.Core.Extensions;
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
using Microsoft.Extensions.Logging.Abstractions;

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
        public static string MaskinportenToken = string.Empty;
        public static bool IsLoggedIn = false;
        private static ILogger _logger;

        public ApplicationManager(ILogger<ApplicationManager> logger = null)
        {
            _logger = logger;
        }

        //public ApplicationManager(NullLogger<Microsoft.Extensions.Logging.ILogger> logger  = null)
        //{
        //    _logger = logger;
        //}


        public void SetEnvironment(IConfigurationRoot applicationConfiguration, IServiceProvider serviceProvider)
        {
            ApplicationConfiguration = applicationConfiguration;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Execute the user command with parameters. Finds the correct service and command handler and starts the service.
        /// </summary>
        /// <param name="args">User input from command line arguments</param>
        public void Execute(string args)
        {
            IsLoggedIn = true;
            if (!string.IsNullOrEmpty(args))
            {
                string[] input = args.ToLower().Split(" ");

                ICommand service = ServiceProvider.GetServices<ICommand>().Where(s => string.Equals(s.Name, input[0], StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (service != null)
                {
                    if (string.Equals(service.Name, "Help", StringComparison.OrdinalIgnoreCase))
                    {
                       service.Run(ParseArguments(input));
                    }
                    else if (IsLoggedIn || (string.Equals(service.Name, "Login", StringComparison.OrdinalIgnoreCase)))
                    {

                        if (input.Length > 1)
                        {
                            ISubCommandHandler commandHandler = processArgs(input);

                            if (commandHandler != null)
                            {
                                if (commandHandler.IsValid)
                                {
                                    if (commandHandler != null && commandHandler.IsValid)
                                    {
                                        service.Run(commandHandler);
                                    }
                                    else if (commandHandler.IsValid)
                                    {
                                        service.Run(ParseArguments(input));
                                    }
                                }
                                else
                                {
                                    _logger.LogError($"Command error: {commandHandler.ErrorMessage}");
                                }
                            }
                            else
                            {
                                service.Run();
                            }
                        }
                        else
                        {
                            service.Run();
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"The command can not be execute, please log in \n");
                    }
                }
                else
                {
                    IHelp helpService = (IHelp)ServiceProvider.GetService<IHelp>();
                    if (service != null)
                    {
                        helpService.GetHelp();
                    }

                    _logger.LogError($"No commands found");
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
                commandHandler.BuildSelectableCommands();
                commandHandler.DictOptions = ParseArguments(input);
                OptionBuilder.Instance(_logger).AssignValueToCliOptions(commandHandler);
                return commandHandler;
            }
            else
            {
                // No command found, find help command to display help.
                IHelp helpService = ApplicationManager.ServiceProvider.GetServices<IHelp>().FirstOrDefault();
                if (helpService != null)
                {
                    ApplicationManager.ServiceProvider.GetServices<IHelp>().FirstOrDefault().GetHelp();
                }
                else
                {
                    _logger.LogError("Help is not found");
                }
            }

            return null;
        }

        /// <summary>
        /// Build Dictionary for input parameters
        /// </summary>
        /// <param name="args">Input parameters split into list of parameter names and values.</param>
        /// <returns></returns>
        private Dictionary<string, string> ParseArguments(string[] args)
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
