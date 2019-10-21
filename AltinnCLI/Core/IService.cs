using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IService
    {
        void Run(ICommandHandler commandHandler = null);

        void Run(Dictionary<string, string> input);

        string Name { get; }
    }
}
