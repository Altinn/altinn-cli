using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IExecutable
    {
        string Name { get; }

        void Execute(string[] args);
    }
}
