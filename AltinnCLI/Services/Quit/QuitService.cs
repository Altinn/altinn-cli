using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services
{
    class QuitService : IService
    {
        void IService.Run(ICommandHandler commandHandler)
        {
            Environment.Exit(0);
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get
            {
                return "Quit";
            }
        }
    }
}
