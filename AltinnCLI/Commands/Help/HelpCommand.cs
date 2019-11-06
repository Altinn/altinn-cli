﻿using AltinnCLI.Core;
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
            
            if (commandHandler != null)
            {

                IHelp item = ServiceProvider.GetServices<IHelp>().Where(s => string.Equals(s.Name, commandHandler.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                item.GetHelp();
                return;
            }

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
            IHelp service = ServiceProvider.GetServices<IHelp>().Where(s => string.Equals(s.Name, input.Keys.ElementAt<string>(1), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if ((service != null) && (service is ICommand))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(service.Name);
                Console.ResetColor();

                Console.WriteLine("\nDESCRIPTION\t{0}", service.Description);

                Console.WriteLine("\nUSAGE\t{0}\n", service.Usage);

                Console.WriteLine("Commands\n");

                List<ISubCommandHandler> items;

                if (input.Count > 2)
                {
                    items = ServiceProvider.GetServices<ISubCommandHandler>()
                            .Where(x => (x is IHelp && string.Equals(x.CommandProvider, service.Name, StringComparison.OrdinalIgnoreCase)) &&
                                        string.Equals(x.Name, input.Keys.ElementAt<string>(2).Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    foreach (IHelp handler in items)
                    { 
                        Console.Write(handler.Name);
                        Console.ResetColor();

                        Console.Write("\t{0}\n", handler.Description);
                        Console.Write("\t{0}\n", handler.Usage);
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

                        Console.Write("\t{0}\n", handler.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("No help, the requested service is not found");
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