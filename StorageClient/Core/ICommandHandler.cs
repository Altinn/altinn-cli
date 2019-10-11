using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface ICommandHandler
    {
        bool Run(string[] args); 

        string Name { get; }
    }
}
