using AltinnCLI.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Finds according to user input chich type of service and which type of command that shall be executed. 
    /// Which is from the ServiceProvider that contains executable services and command handlers
    /// </summary>
    public class ApplicationManager
    {

        public static IConfigurationRoot ApplicationConfiguration;
        public static IServiceProvider ServiceProvider;

        public ApplicationManager(IServiceProvider serviceProvider, IConfigurationRoot applicationConfiguration)
        {
            ServiceProvider = serviceProvider;
            ApplicationConfiguration = applicationConfiguration;
        }

        /// <summary>
        /// Execute the user command with parameters. Consists in finding correct service and command handler and start service.
        /// </summary>
        /// <param name="args">input command line arguments</param>
        public void Execute(string args)
        {
            if (!string.IsNullOrEmpty(args))
            {
                string[] input = args.ToLower().Split(" ");

                IService service = ServiceProvider.GetServices<IService>().Where(s => string.Equals(s.Name, input[0], StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (service != null)
                {
                    if (input.Length > 1)
                    {
                        ICommandHandler commandHandler = processArgs(input);
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
        /// Finds according to input parameters the service and commmand handler that shall execute the command .
        /// It co service
        /// </summary>
        /// <param name="input">command line arguments that is split into list of parameter separated by space</param>
        /// <returns>A commandhanlder that shall execute the used input command</returns>
        private ICommandHandler processArgs(string[] input)
        {
            ICommandHandler commandHandler = ApplicationManager.ServiceProvider.GetServices<ICommandHandler>()
                .FirstOrDefault(s => string.Equals(s.Name, input[1], StringComparison.OrdinalIgnoreCase) && 
                string.Equals(s.ServiceProvider, input[0],StringComparison.OrdinalIgnoreCase));

            if (commandHandler != null)
            {
                commandHandler.CommandParameters = ParseArguments(input);
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
        /// <param name="args">input parameters split into list of strings. Space has been used as split char</param>
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
