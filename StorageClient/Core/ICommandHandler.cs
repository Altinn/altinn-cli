using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface ICommandHandler
    {
        bool Run(); 

        string Name { get; }

        List<KeyValuePair<string,string>> CommandParameters { get; set; }
    }
}
