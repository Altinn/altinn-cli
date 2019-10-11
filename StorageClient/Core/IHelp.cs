using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public interface IHelp
    {
        string Name { get; }

        string GetHelp();
    }
}
