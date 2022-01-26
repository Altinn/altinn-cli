using AltinnCLI.Commands.Core;
using System;
using System.Collections.Generic;

namespace AltinnCLI.Commands.Prefill
{
    public class PrefillCommand : ICommand, IHelp
    {
        public string Name => "Prefill";

        public string Description => $"\tProvides different prefill options.";

        public string Usage => $@"Prefill <command>";

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public void Run(ISubCommandHandler commandHandler = null)
        {
            if (commandHandler == null)
            {
                Console.WriteLine();
                Console.WriteLine(Usage);
                Console.WriteLine();
            }
            else
            {
                commandHandler.Run();
            }
        }

        public void Run(Dictionary<string, string> input)
        {
            Console.WriteLine();
            Console.WriteLine(Usage);
            Console.WriteLine();
        }
    }
}
