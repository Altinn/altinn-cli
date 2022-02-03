using System;
using System.Collections.Generic;
using System.Linq;

using AltinnCLI.Commands.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static string MaskinportenToken = string.Empty;
        public static bool IsLoggedIn = false;
        private readonly ILogger _logger;

        public ApplicationManager(ILogger<ApplicationManager> logger = null)
        {
            _logger = logger;
        }

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
            if (!string.IsNullOrEmpty(args))
            {
                string[] input = args.ToLower().Split(" ");

                ICommand command = ServiceProvider.GetServices<ICommand>().FirstOrDefault(s => string.Equals(s.Name, input[0], StringComparison.OrdinalIgnoreCase));

                if (command != null)
                {
                    if (string.Equals(command.Name, "Help", StringComparison.OrdinalIgnoreCase))
                    {
                        command.Run(ParseArguments(input));
                    }
                    else if (string.Equals(command.Name, "Quit", StringComparison.OrdinalIgnoreCase))
                    {
                        command.Run();
                    }
                    else 
                    {

                        if (input.Length > 1)
                        {
                            RunCommandWithParameters(input, command);
                        }
                        else
                        {
                            command.Run();
                        }
                    }

                    //to do: handle authentication for all actions
                   /* else
                    {
                        _logger.LogInformation($"The command can not be execute, please log in \n");
                    }*/ 
                }
                else
                {
                    _logger.LogError($"No commands found");
                }
            }
        }

        private void RunCommandWithParameters(string[] input, ICommand command)
        {
            ISubCommandHandler subCommandHandler = ProcessArgs(input);

            if (subCommandHandler != null)
            {
                if (subCommandHandler.IsValid)
                {
                    if (subCommandHandler.IsValid)
                    {
                        command.Run(subCommandHandler);
                    }
                    else if (subCommandHandler.IsValid)
                    {
                        command.Run(ParseArguments(input));
                    }
                }
                else
                {
                    _logger.LogError($"Command error: {subCommandHandler.ErrorMessage}");
                }
            }
            else
            {
                command.Run();
            }
        }

        /// <summary>
        /// Parses input parameters and finds the correct service and command handler for executing the command.
        /// </summary>
        /// <param name="input">
        /// User input from command line arguments, split into list of parameters separated by space. 
        /// Commands are structured as follows: [command] [subCommand] [options]
        /// </param>
        /// <returns>A commandHandler that can execute the command given by user input</returns>
        private ISubCommandHandler ProcessArgs(string[] input)
        {
            ISubCommandHandler subCommandHandler = ServiceProvider.GetServices<ISubCommandHandler>()
                .FirstOrDefault(s =>
                    string.Equals(s.Name, input[1], StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(s.CommandProvider, input[0], StringComparison.OrdinalIgnoreCase));

            if (subCommandHandler != null)
            {
                subCommandHandler.BuildSelectableCommands();
                subCommandHandler.DictOptions = ParseArguments(input);
                OptionBuilder.Instance(_logger).AssignValueToCliOptions(subCommandHandler);
                return subCommandHandler;
            }

            // No command found, find help command to display help.
            IHelp helpService = ServiceProvider.GetServices<IHelp>().FirstOrDefault();
            if (helpService != null)
            {
                helpService.GetHelp();
            }
            else
            {
                _logger.LogError("Help is not found");
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
            Dictionary<string, string> commandKeysAndValues = new();

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
