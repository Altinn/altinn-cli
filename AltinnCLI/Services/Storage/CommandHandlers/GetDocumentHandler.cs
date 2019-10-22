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
    /// Commandhandler that is used to fetch documents from ALtinn Blob storage.  
    /// </summary>
    public class GetDocumentHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDocumentHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public GetDocumentHandler(ILogger<GetDocumentHandler> logger) : base(logger)
        {

            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                ClientWrapper = new StorageClientWrapper();
            }
            else
            {
                ClientWrapper = new StorageClientFileWrapper();
            }

        }

        /// <summary>
        /// Gets the name of of the command
        /// </summary>
        public string Name
        { 
            get
            {
                return "GetDocument";
            }
        }

        /// <summary>
        /// Gets the description of the command handler that is used by the Help function
        /// </summary>
        public string Description
        {
            get
            {
                return "Returns a specific document from Storage";
            }
        }

        /// <summary>
        /// Gets how the command can be specified to get documents, is used by the Help function
        /// </summary>
        public string Usage
        {
            get
            {
                return  $"Storage GetDocument  -Fetch all documents from storage \n" +
                        $"Storage GetDocument ownerid=<id>  -Fetch all documents from owner \n" +
                        $"Storage GetDocument ownerid=<id> instanceId=<instance-guid> documentId=<document-guid> -Fetch specific document \n";
            }
        }

        /// <summary>
        /// Gets the name of the ServiceProvider that uses this command
        /// </summary>
        public string ServiceProvider
        {
            get
            {
                return "Storage";
            }
        }

        /// <summary>
        /// Gets the validation status for the command parameters
        /// </summary>
        public bool IsValid 
        {
            get
            {
                return Validate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return Name;
        }

        /// <summary>
        /// Processes the command
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (IsValid)
            {
                int? ownerId = CommandParameters.GetValueOrDefault("ownerid") != null ? int.Parse(CommandParameters.GetValueOrDefault("ownerid")) : (int?)null;
                Guid? instanceId = CommandParameters.GetValueOrDefault("instanceid") != null ? Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")) : (Guid?)null;
                Guid? dataId = CommandParameters.GetValueOrDefault("dataid") != null ? Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")) : (Guid?)null;
                string updateInstances = CommandParameters.GetValueOrDefault("updateinstances") != null ? CommandParameters.GetValueOrDefault("instanceid") : string.Empty;

                if ((ownerId != null) && (instanceId != null) && (dataId != null))
                {
                    Stream stream = ClientWrapper.GetDocument((int)ownerId, (Guid)instanceId, (Guid)dataId);

                    if (stream != null)
                    {
                        string fileFolder = $@"{ownerId}\{instanceId}";

                        SaveToFile(fileFolder, ((Guid)dataId).ToString(), stream);
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

        /// <summary>
        /// Fetch instance data and call member method to fetch and save document on file
        /// </summary>
        /// <param name="ownerId">The owner of the documents that shall be fetched</param>
        /// <param name="instanceId">The application instance for which the documents shall be fetched</param>
        /// <param name="updateInstances">Defines if the instances shall be marked to indicate that the documents is read </param>
        private void GetDocumentFromInstances(int? ownerId, Guid? instanceId, string updateInstances)
        {
            InstanceResponseMessage responsMessage = ClientWrapper.GetInstanceMetaData(ownerId, instanceId);
            _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

            Instance[] instances = responsMessage.Instances;
            FetchAndSaveDocuments(instances, responsMessage.Next, updateInstances);

        }


        /// <summary>
        ///  Fetch document from storage and save it to file. If the Next link is defined continue to read rest of instances and documents
        /// </summary>
        /// <param name="instances">The insatnces for which the documents shall be fetched</param>
        /// <param name="nextLink">The fetch of documents are paged, the next link shall be used to fetch next page of instances</param>
        /// <param name="updateInstances"></param>
        private void FetchAndSaveDocuments(Instance[] instances, Uri nextLink, string updateInstances)
        {
            foreach (Instance instance in instances)
            {
                foreach (DataElement data in instance.Data)
                { 
                    string url = data.DataLinks.Platform;
                    Stream responsData = ClientWrapper.GetDocument(url, data.ContentType);

                    if (responsData != null)
                    {
                        string instanceGuidId = instance.Id.Split('/')[1];
                        string fileName = $"{data.ElementType.ToString()}_{((string.IsNullOrEmpty(data.FileName)) ? data.Id : data.FileName)}";

                        string fileFolder = $@"{instance.InstanceOwnerId}\{instanceGuidId}";

                        SaveToFile(fileFolder, fileName, responsData);
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

        /// <summary>
        /// Verifies if the input parameters are valid.
        /// </summary>
        /// <returns></returns>
        protected bool Validate()
        {
            if (HasParameter("ownerid"))
            {
                if (!HasParameterWithValue("ownerid"))
                {
                    _logger.LogError("Missing parameter value for OwnerId");
                    return false;
                }

                if (HasParameter("instanceid"))
                {
                    if (!HasParameterWithValue("instanceid"))
                    {
                        _logger.LogError("Missing paramterer value for InstanceId");
                        return false;
                    }

                    if (HasParameter("dataId"))
                    {
                        if (!HasParameterWithValue("dataId"))
                        {
                            _logger.LogError("Missing paramterer value for dataId");
                            return false;
                        }

                    }
                }
            }
            return true;
        }
    }
}
