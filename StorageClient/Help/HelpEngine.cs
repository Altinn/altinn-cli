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
            GetHelp();
        }

        public string GetHelp()
        {
            return "Help";
        }


        public string Name
        {
            get
            {
                return "Help";
            }
        }
    }
}
