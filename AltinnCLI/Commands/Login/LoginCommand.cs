using System;
using System.Collections.Generic;

using AltinnCLI.Core;

namespace AltinnCLI.Commands.Login
{
    class LoginCommand : ICommand, IHelp
    {
        public string Name => "Login";

        public string Description => $"\tProvides different login options.";

        public string Usage => $@"Login <command>";

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public void Run(ISubCommandHandler commandHandler = null)
        {
            Console.WriteLine();
            Console.WriteLine(Usage);
            Console.WriteLine();
        }

        public void Run(Dictionary<string, string> input)
        {
            Console.WriteLine();
            Console.WriteLine(Usage);
            Console.WriteLine();
        }
    }
}
