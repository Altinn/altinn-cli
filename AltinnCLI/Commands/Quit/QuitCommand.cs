using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Commands
{
    class QuitCommand : ICommand
    {
        void ICommand.Run(ISubCommandHandler commandHandler)
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
