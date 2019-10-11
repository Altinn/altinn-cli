using AltinnCLI.Core;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Storage
{
    public class GetDocumentHandler : ICommandHandler, IHelp
    {
        public string Name
        { 
            get
            {
                return "GetCommand";
            }
        }

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public bool Run(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
