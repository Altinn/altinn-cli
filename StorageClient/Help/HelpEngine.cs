using AltinnCli;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    class HelpService : IService, IHelp
    {
        public void Run(string[] args)
        {
            
        }

        public string GetHelp()
        {
            return "Help";
        }
    }
}
