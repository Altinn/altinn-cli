using AltinnCLI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltinnCLI.Commands
{
    class HelpCommand : ICommand, IHelp
    {
        private IServiceProvider ServiceProvider;

        public HelpCommand()
        {
            ServiceProvider = ApplicationManager.ServiceProvider;
        }


        public void Run(ISubCommandHandler commandHandler = null)
        {
            AllCommandsHelp();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> CliOptions { get; set; }

        public string GetHelp()
        {
            return "Altinn CLI - A command line interface for managing your Altinn Applications\n\nCOMMANDS:\n\n";
        }

        public bool Run()
        {
            throw new NotImplementedException();
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
                IHelp service = ServiceProvider.GetServices<IHelp>().Where(s => string.Equals(s.Name, input.Keys.ElementAt<string>(1), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if ((service != null) && (service is ICommand))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(service.Name);
                    Console.ResetColor();

                    Console.WriteLine($"\nDESCRIPTION\t{service.Description}");

                    Console.WriteLine($"\nUSAGE\t{service.Usage}\n");

                    Console.WriteLine($"Commands\n");

                    List<ISubCommandHandler> items;

                    if (input.Count > 2)
                    {
                        items = ServiceProvider.GetServices<ISubCommandHandler>()
                                .Where(x => (x is IHelp && string.Equals(x.CommandProvider, service.Name, StringComparison.OrdinalIgnoreCase)) &&
                                            string.Equals(x.Name, input.Keys.ElementAt<string>(2).Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
//                        Console.ForegroundColor = ConsoleColor.Yellow;

                        if (items != null && items.Count > 0)
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
                        items = ServiceProvider.GetServices<ISubCommandHandler>().
                                Where(x => x is IHelp && string.Equals(x.CommandProvider, service.Name, StringComparison.OrdinalIgnoreCase)).ToList();

                        foreach (IHelp handler in items)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(handler.Name);
                            Console.ResetColor();

                            Console.Write($"\t{handler.Description}\n");
                        }
                    }
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

            List<IHelp> items = ServiceProvider.GetServices<IHelp>().Where(x => x.GetType().BaseType.IsAssignableFrom(typeof(ICommand))).ToList();

            foreach (IHelp item in items)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(item.Name);
                Console.ResetColor();

                Console.Write($"\t{item.Description}\n");

            }
        }

        public string Name
        {
            get
            {
                return "Help";
            }
        }

        public string Description
        {
            get
            {
                return "\tCommands for getting help";
            }
        }

        public string Usage
        {
            get
            {
                return "Usage";
            }
        }

    }
}
