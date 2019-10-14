using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    /// <summary>
    /// Parameters, 
    /// </summary>
    public class GetDocumentHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;
        
        public GetDocumentHandler()
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                ClientWrapper = new StorageClientWrapper();
            }

        }

        /// <summary>
        /// Required parameters: 
        /// int OwnerId 
        /// Guid InstanceId
        /// Guid DataId
        /// </summary>
        public string Name
        { 
            get
            {
                return "GetDocument";
            }
        }

        public string Description
        {
            get
            {
                return "Returns a specific document from Storage";
            }
        }

        public string Usage
        {
            get
            {
                return "Storage GetDocument -documentId=<document-guid>";
            }
        }

        public string ServiceProvider
        {
            get
            {
                return "Storage";
            }
        }

        public bool IsValid 
        {
            get
            {
                return Validate();
            }
        }

        public string GetHelp()
        {
            return Name;
        }

        public bool Run()
        {
            ClientWrapper.GetDocument(int.Parse(CommandParameters.GetValueOrDefault("OwnerId")),
                                      Guid.Parse(CommandParameters.GetValueOrDefault("InstanceId")),
                                      Guid.Parse(CommandParameters.GetValueOrDefault("DataId")));

            //documentStream = wrapper.GetDocument(instanceOwnerId, instanceGuid, dataId);
            return true;
        }



        protected bool Validate()
        {
            return (HasParameterWithValue("OwnerId") & HasParameterWithValue("InstanceId") & HasParameterWithValue("DataId"));
        }
    }
}
