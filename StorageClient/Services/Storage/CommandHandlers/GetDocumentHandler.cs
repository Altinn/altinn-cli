﻿using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StorageClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    /// <summary>
    /// Parameters, 
    /// </summary>
    public class GetDocumentHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;
        private readonly ILogger _logger;

        public GetDocumentHandler(ILogger<GetDocumentHandler> logger)
        {
            _logger = logger;

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
            if (IsValid)
            {
                int ownerId = int.Parse(CommandParameters.GetValueOrDefault("ownerid"));
                Guid instanceId = Guid.Parse(CommandParameters.GetValueOrDefault("instanceid"));
                Guid dataId = Guid.Parse(CommandParameters.GetValueOrDefault("dataid"));

                Stream stream = ClientWrapper.GetDocument(ownerId, instanceId, dataId);

                if (stream != null)
                {
                    SaveToFile(ownerId, instanceId, dataId, stream);
                }

            }
            else
            {
                _logger.LogInformation("Missing parameters");
            }
            return true;
        }

        protected bool Validate()
        {
            return (HasParameterWithValue("ownerid") & HasParameterWithValue("instanceid") & HasParameterWithValue("dataid"));
        }
    }
}
