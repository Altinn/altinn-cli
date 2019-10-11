using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IService
    {
        void Run(string[] args);

        string Name { get; }
    }
}
