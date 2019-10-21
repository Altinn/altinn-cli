using AltinnCLI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltinnCLI.Services
{
    class HelpService : IService, IHelp
    {
        private IServiceProvider ServiceProvider;

        public HelpService()
        {
            ServiceProvider = ApplicationManager.ServiceProvider;
        }


        public void Run(ICommandHandler commandHandler = null)
        {
            
            if (commandHandler != null)
            {

                IHelp item = ServiceProvider.GetServices<IHelp>().Where(s => string.Equals(s.Name, commandHandler.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                item.GetHelp();
                return;
            }

           Console.WriteLine(GetHelp());

            foreach (IHelp item in ServiceProvider.GetServices<IHelp>())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(item.Name);
                Console.ResetColor();

                Console.WriteLine("\nDESCRIPTION\t{0}", item.Description);

                Console.WriteLine("\nUSAGE\t{0}\n\n", item.Usage);

            }
        }

        public string GetHelp()
        {
            return "Altinn CLI - A command line interface for managing your Altinn Applications\n\nCOMMANDS:\n\n";
        }

        public bool Run()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Assume that the input parameter order is  help GetDocument 
        /// </summary>
        /// <param name="input"></param>
        public void Run(Dictionary<string, string> input)
        {
            IHelp service = ServiceProvider.GetServices<IHelp>().Where(s => string.Equals(s.Name, input.Keys.ElementAt<string>(1), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(service.Name);
            Console.ResetColor();

            Console.WriteLine("\nDESCRIPTION\t{0}", service.Description);

            Console.WriteLine("\nUSAGE\t{0}\n\n", service.Usage);

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
                return "Help";
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
