using AltinnCli;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    class HelpService : IService, IHelp
    {
        private ServiceProvider ServiceProvider;

        public HelpService(ServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }


        public void Run(string[] args)
        {
            GetHelp();
        }

        public string GetHelp()
        {
            return "Help";
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
