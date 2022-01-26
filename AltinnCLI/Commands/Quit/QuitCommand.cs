using System;
using System.Collections.Generic;
using AltinnCLI.Commands.Core;

namespace AltinnCLI.Commands.Quit
{
    class QuitCommand : ICommand, IHelp
    {
        public string Name => "Quit";

        public string Description => "\tCommand for exiting Altinn CLI.";

        public string Usage => "Quit";

        public string GetHelp()
        {
            return "Quit";
        }

        public void Run(ISubCommandHandler commandHandler = null)
        {
            Environment.Exit(0);
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }
    }
}
