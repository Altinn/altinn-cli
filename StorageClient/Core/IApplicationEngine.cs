using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCli
{
    public interface IApplicationEngine
    {
        void Execute(string[] args);

        string Name { get; }
    }
}
