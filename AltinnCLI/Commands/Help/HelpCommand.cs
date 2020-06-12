using System;
using System.Collections.Generic;
using System.Linq;

using AltinnCLI.Core;

using Microsoft.Extensions.DependencyInjection;

namespace AltinnCLI.Commands.Help
{
    class HelpCommand : ICommand, IHelp
    {
        private readonly IServiceProvider _serviceProvider;

        public HelpCommand()
        {
            _serviceProvider = ApplicationManager.ServiceProvider;
        }

        public string Name => "Help";

        public string Description => "\tCommand for getting help on a command";

        public string Usage => "Usage";

        public string GetHelp()
        {
            return "Altinn CLI - A command line interface for managing your Altinn Applications\n\nCOMMANDS:\n";
        }

        public void Run(ISubCommandHandler commandHandler = null)
        {
            AllCommandsHelp();
        }

        /// <summary>
        /// Assume that the input parameter order is  help GetData 
        /// </summary>
        /// <param name="input"></param>
        public void Run(Dictionary<string, string> input)
        {
            if (input.Count == 1)
            {
                AllCommandsHelp();
            }
            else
            {
                IHelp service = _serviceProvider.GetServices<IHelp>()
                    .FirstOrDefault(s => string.Equals(s.Name, input.Keys.ElementAt(1), StringComparison.OrdinalIgnoreCase));

                if (service is ICommand)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(service.Name);
                    Console.ResetColor();

                    Console.WriteLine($"\nDESCRIPTION\t{service.Description}");

                    Console.WriteLine($"\nUSAGE\t{service.Usage}\n");

                    Console.WriteLine($"Commands:\n");

                    List<ISubCommandHandler> items;

                    if (input.Count > 2)
                    {
                        items = _serviceProvider.GetServices<ISubCommandHandler>()
                            .Where(x =>
                                x is IHelp &&
                                string.Equals(x.CommandProvider, service.Name, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(x.Name, input.Keys.ElementAt(2).Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

                        if (items.Count > 0)
                        {
                            foreach (IHelp handler in items)
                            {
                                ((ISubCommandHandler)handler).BuildSelectableCommands();
                                Console.Write(handler.Name);
                                Console.ResetColor();

                                Console.Write($"\t{handler.Description}\n");
                                Console.Write($"\t{handler.Usage}\n");
                            }
                        }
                        else
                        {
                            Console.Write($"No help for specified options\n");
                        }
                    }
                    else
                    {
                        items = _serviceProvider.GetServices<ISubCommandHandler>().Where(x =>
                            x is IHelp && 
                            string.Equals(x.CommandProvider, service.Name, StringComparison.OrdinalIgnoreCase)).ToList();

                        foreach (IHelp handler in items)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(handler.Name);
                            Console.ResetColor();

                            Console.Write($"\t{handler.Description}\n");
                        }
                    }

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No help, the requested service is not found");
                }
            }
        }

        private void AllCommandsHelp()
        {
            Console.WriteLine(GetHelp());

            List<IHelp> items = _serviceProvider.GetServices<IHelp>().Where(x => x.GetType().BaseType.IsAssignableFrom(typeof(ICommand))).ToList();

            foreach (IHelp item in items)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(item.Name);
                Console.ResetColor();

                Console.Write($"\t{item.Description}\n");
            }

            Console.WriteLine();
        }
    }
}
