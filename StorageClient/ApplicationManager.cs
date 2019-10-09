using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public class ApplicationManager
    {

        private string _args { get; set; }

        public ApplicationManager(string args)
        {
            _args = args;
        }

        private string[] processArgs()
        {
            return _args.Split(" ");
        }
    }
}
