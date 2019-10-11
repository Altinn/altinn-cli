using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IHelp
    {
        string Name { get; }

        string GetHelp();
    }
}
