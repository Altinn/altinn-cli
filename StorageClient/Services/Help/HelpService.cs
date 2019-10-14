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
            // commandHandler.Run();
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
