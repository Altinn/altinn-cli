using AltinnCLI.Core;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public class GetInstanceHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        public string Name
        {
            get
            {
                return "GetInstance";
            }
        }

        public string Description
        {
            get
            {
                return "Returns the metedata for an instance of an Application";
            }
        }

        public string Usage
        {
            get
            {
                return "AltinnCLI > storage GetInstance -instanceId=<instance-guid>";
            }
        }

        public string GetHelp()
        {
            return Name;
        }

        public bool Run()
        {
            throw new NotImplementedException();
        }
    }
}
