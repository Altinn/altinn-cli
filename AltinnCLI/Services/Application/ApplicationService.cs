using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services.Application
{
    class ApplicationService : IService, IHelp
    {
        private IServiceProvider ServiceProvider;

        public ApplicationService()
        {
            ServiceProvider = ApplicationManager.ServiceProvider;
        }

        public string Name
        {
            get
            {
                return "Application";
            }
        }

        public string Description
        {
            get
            {
                return "Tools for creating an application";
            }
        }

        public string Usage
        {
            get
            {
                return "DO NOT USE!";
            }
        }

        public string GetHelp()
        {
            return "";
        }

        public virtual void Run(ICommandHandler commandHandler)
        {
            if (commandHandler != null)
            {
                commandHandler.Run();
            }
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }
    }
}
