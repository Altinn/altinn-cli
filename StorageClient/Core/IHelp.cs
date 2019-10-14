using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IHelp
    {
        string Name { get; }

        string Description { get; }

        string Usage { get; }

        string GetHelp();
    }
}
