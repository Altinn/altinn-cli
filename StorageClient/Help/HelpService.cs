using AltinnCli;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorageClient
{
    class HelpService : IService, IHelp
    {
        private IServiceProvider ServiceProvider;

        public HelpService()
        {
            ServiceProvider = ApplicationManager.ServiceProvider;
        }


        public void Run(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine(ServiceProvider.GetServices<IHelp>().Where(s => string.Equals(s.Name, args[1], StringComparison.OrdinalIgnoreCase)).Single().GetHelp());
            }
            else
            {
                foreach (IHelp item in ServiceProvider.GetServices<IHelp>())
                {
                    Console.WriteLine(item.GetHelp());
                }
            }
        }

        public string GetHelp()
        {
            return "Altinn CLI - A command line interface for managing your Altinn Applications\n\nCOMMANDS:\n\n";
        }


        public string Name
        {
            get
            {
                return "Help";
            }
        }
    }
}
