using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public class GetDocumentHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        public string Name
        { 
            get
            {
                return "GetDocument";
            }
        }

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public bool Run()
        {
            string baseAddress = ApplicationManager.ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            bool useLiveClient = ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>();

            //documentStream = wrapper.GetDocument(instanceOwnerId, instanceGuid, dataId);
            return true;
        }
    }
}
