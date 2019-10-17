using Altinn.Platform.Storage.Models;
using AltinnCLI.Core;
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

        public GetDocumentHandler(ILogger<GetDocumentHandler> logger) : base(logger)
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
            int? ownerId = CommandParameters.GetValueOrDefault("ownerid") != null ? int.Parse(CommandParameters.GetValueOrDefault("ownerid")) : (int?)null;
            Guid? instanceId = CommandParameters.GetValueOrDefault("instanceid") != null ? Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")) : (Guid?)null;
            Guid? dataId = CommandParameters.GetValueOrDefault("dataid") != null ? Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")) : (Guid?)null;
            string updateInstances = CommandParameters.GetValueOrDefault("updateinstances") != null ? CommandParameters.GetValueOrDefault("instanceid") : string.Empty;

            if (IsValid)
            {
                if ((ownerId != null) && (instanceId != null) && (dataId != null))
                {
                    Stream stream = ClientWrapper.GetDocument((int)ownerId, (Guid)instanceId, (Guid)dataId);

                    if (stream != null)
                    {
                        SaveToFile((int)ownerId, (Guid)instanceId, ((Guid)dataId).ToString(), stream);
                    }
                }
                else
                {
                    GetDocumentFromInstances(ownerId, instanceId, updateInstances);
                }


            }
            else
            {
                _logger.LogInformation("Missing parameters");
            }
            return true;
        }

        private void GetDocumentFromInstances(int? ownerId, Guid? instanceId, string updateInstances)
        {
            InstanceResponseMessage responsMessage = ClientWrapper.GetInstanceMetaData(ownerId, instanceId);
            _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

            Instance[] instances = responsMessage.Instances;
            FetchAndSaveDocuments(instances, responsMessage.Next, updateInstances);

        }

        private void FetchAndSaveDocuments(Instance[] instances, Uri nextLink, string updateInstances)
        {
            foreach (Instance instance in instances)
            {
                foreach (DataElement data in instance.Data)
                { 
                    string url = data.DataLinks.Platform;
                    Stream responsData = ClientWrapper.GetDocument(url);

                    if (responsData != null)
                    {
                        string instanceGuidId = instance.Id.Split('/')[1];
                        string fileName = $"{data.ElementType.ToString()}_{((string.IsNullOrEmpty(data.FileName)) ? data.Id : data.FileName)}";

                        SaveToFile(int.Parse(instance.InstanceOwnerId), Guid.Parse(instanceGuidId), fileName, responsData);
                    }
                }
            }


            if (nextLink != null)
            {
                InstanceResponseMessage responsMessage = ClientWrapper.GetInstanceMetaData(nextLink);
                _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

                FetchAndSaveDocuments(responsMessage.Instances, responsMessage.Next, updateInstances);
            }
        }

        protected bool Validate()
        {
            return true;
//            return (HasParameterWithValue("ownerid") & HasParameterWithValue("instanceid") & HasParameterWithValue("dataid"));
        }
    }
}
