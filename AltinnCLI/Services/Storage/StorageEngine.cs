using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltinnCLI.Services.Storage
{
    public class StorageEngine : IService, IHelp
    {
        public StorageEngine()
        {
        }


        public virtual void Run(ICommandHandler commandHandler)
        {
            if (commandHandler != null)
            {
                commandHandler.Run();
            }
        }

        public string Name
        {
            get
            {
                return "Storage";
            }
        }

        public string Description
        {
            get
            {
                return "Commands for interacting with the Storage";
            }
        }

        public string Usage
        {
            get
            {
                return "storage <operation> -<option>";
            }
        }

        public string GetHelp()
        {
            return "Storage\nusage: storage <operation> -<option>\n\noperations:\ngetAttachment";
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }
    }
}
