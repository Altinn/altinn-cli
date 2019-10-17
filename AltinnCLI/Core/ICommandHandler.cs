using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface ICommandHandler
    {
        bool Run(); 

        string Name { get; }

        string ServiceProvider { get; }

        Dictionary<string,string> CommandParameters { get; set; }

        bool IsValid { get; }
    }
}
