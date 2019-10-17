using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IApplicationEngine
    {
        void Execute(string[] args);

        string Name { get; }
    }
}
