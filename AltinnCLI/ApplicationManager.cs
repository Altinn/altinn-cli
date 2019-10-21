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
    public class ApplicationManager
    {

        public static IConfigurationRoot ApplicationConfiguration;
        public static IServiceProvider ServiceProvider;

        public ApplicationManager(IServiceProvider serviceProvider, IConfigurationRoot applicationConfiguration)
        {
            ServiceProvider = serviceProvider;
            ApplicationConfiguration = applicationConfiguration;
        }


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
