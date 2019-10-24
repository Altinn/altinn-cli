using Altinn.Platform.Storage.Models;
using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AltinnCLI.Services.Storage
{
    /// <summary>
    /// Commandhandler that is used to fetch documents from Altinn Blob storage.  
    /// </summary>
    public class GetDocumentHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;

        private int? ownerId = null;
        private Guid? instanceId;
        private Guid? dataId;
        private string updateInstances = string.Empty;
        private string org = string.Empty;
        private string appId = string.Empty;
        private string currentTaskId = string.Empty;
        private bool? processIsComplete = null;
        private bool? processIsInError = null;
        private string processEndState = string.Empty;

        private string lastChangedDateTime = string.Empty;
        private string createdDateTime = string.Empty;
        private string visibleDateTime = string.Empty;
        private string dueDateTime = string.Empty;

        private string continuationToken = string.Empty;
        private int? size = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDocumentHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public GetDocumentHandler(ILogger<GetDocumentHandler> logger) : base(logger)
        {

            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                ClientWrapper = new StorageClientWrapper(_logger);
            }
            else
            {
                ClientWrapper = new StorageClientFileWrapper(_logger);
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
                return  $"Storage GetDocument org=<org> appid=<appid> processIsComplete=<true/fals> lastChangedDate=<gt:2019-10-23>  -Fetch documents for org or app that is completed for the org/appId since lastchangeddate \n" +
                        $"Use either app or appid \n" +
                        $" Available parameters for the command that download documents for an application \n" + 
                        $"  org -application\n" +
                        $"  appId -application id\n" +
                        $"  currentTaskId -running process current task id\n" +
                        $"  processIsComplete -is process complete\n" +
                        $"  processIsInError -is process in error\n" +
                        $"  processEndState -process end state\n" +
                        $"  labels -labels\n" +
                        $"  lastChangedDateTime -last changed date \n" +
                        $"  createdDateTime -created time \n"+
                        $"  visibleDateTime -the visible date time \n" +
                        $"  dueDateTime -the due date time \n" +
                        $"  continuationToken -continuation token \n" +
                        $"  size the page size \n" +
                        $"Storage GetDocument  -Fetch all documents from storage \n" +
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
                SetLocals();

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
                    GetDocumentFromInstances();
                }


            }
            else
            {
                _logger.LogInformation("Missing parameters");
            }
            return true;
        }

        private void SetLocals()
        {
            ownerId = CommandParameters.GetValueOrDefault("ownerid") != null ? int.Parse(CommandParameters.GetValueOrDefault("ownerid")) : (int?)null;
            instanceId = CommandParameters.GetValueOrDefault("instanceid") != null ? Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")) : (Guid?)null;
            dataId = CommandParameters.GetValueOrDefault("dataid") != null ? Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")) : (Guid?)null;
            updateInstances = CommandParameters.GetValueOrDefault("updateinstances") != null ? CommandParameters.GetValueOrDefault("instanceid") : string.Empty;

            appId = CommandParameters.GetValueOrDefault("appid") != null ? CommandParameters.GetValueOrDefault("appid") : string.Empty;
            org = CommandParameters.GetValueOrDefault("org") != null ? CommandParameters.GetValueOrDefault("org") : string.Empty;

            processIsComplete = CommandParameters.GetValueOrDefault("processsscomplete") != null ? bool.Parse(CommandParameters.GetValueOrDefault("processIsComplete")) : false;
            processIsInError = CommandParameters.GetValueOrDefault("processisisError") != null ? bool.Parse(CommandParameters.GetValueOrDefault("processisinerror")) : false;
            processEndState = CommandParameters.GetValueOrDefault("processendstate") != null ? CommandParameters.GetValueOrDefault("processendstate") : string.Empty;

            lastChangedDateTime = CommandParameters.GetValueOrDefault("lastchangedatetime") != null ? CommandParameters.GetValueOrDefault("lastchangeddatetime") : string.Empty;
            createdDateTime = CommandParameters.GetValueOrDefault("createddatetime") != null ? CommandParameters.GetValueOrDefault("createdtatetime") : string.Empty;
            visibleDateTime = CommandParameters.GetValueOrDefault("visibledatetime") != null ? CommandParameters.GetValueOrDefault("visibledatetime") : string.Empty;
            dueDateTime = CommandParameters.GetValueOrDefault("duedatetime") != null ? CommandParameters.GetValueOrDefault("duedatetme") : string.Empty;

            continuationToken = CommandParameters.GetValueOrDefault("continuationToken") != null ? CommandParameters.GetValueOrDefault("continuationToken") : string.Empty;
            size = CommandParameters.GetValueOrDefault("size") != null ? int.Parse(CommandParameters.GetValueOrDefault("size")) : (int?)null;
        }

        /// <summary>
        /// Fetch instance data and call member method to fetch and save document on file
        /// </summary>
        /// <param name="ownerId">The owner of the documents that shall be fetched</param>
        /// <param name="instanceId">The application instance for which the documents shall be fetched</param>
        /// <param name="updateInstances">Defines if the instances shall be marked to indicate that the documents is read </param>
        private void GetDocumentFromInstances()
        {
            InstanceResponseMessage responsMessage = null;

            if (!string.IsNullOrEmpty(appId) || !string.IsNullOrEmpty(org))
            {
                responsMessage = ClientWrapper.GetInstanceMetaData(appId, CommandParameters);

            }
            else if (ownerId != null)
            {
                responsMessage = ClientWrapper.GetInstanceMetaData(ownerId, instanceId);
            }

            if (responsMessage != null)
            {
                _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

                Instance[] instances = responsMessage.Instances;
                FetchAndSaveDocuments(instances, responsMessage.Next, updateInstances);
            }

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

            if (HasParameter(org) && HasParameter(appId))
            {
                _logger.LogError("Oly org or app can be defined as parameter");
                return false;
            }

            if (HasParameter(org))
            {
                if (!HasParameterWithValue("org"))
                {
                    _logger.LogError("Missing parameter value for org");
                    return false;
                }

                return CheckInstanceOptions();
            }

            if (HasParameter(appId))
            {
                if (!HasParameterWithValue("appid"))
                {
                    _logger.LogError("Missing parameter value for appId");
                    return false;
                }

                return CheckInstanceOptions();
            }

            return true;
        }

        private bool CheckInstanceOptions()
        {
            if (HasParameter("currenttaskid"))
            {
                if (!HasParameterWithValue("currenttaskid"))
                {
                    _logger.LogError("Wrong or missing parameter value for currenttaskid");
                    return false;
                }
            }

            if (HasParameter("processiscomplete"))
            {
                if (!HasParameterWithValue("processiscomplete"))
                {
                    _logger.LogError("Wrong or missing parameter value for processiscomplete");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("processisinerror"))
                {
                    _logger.LogError("Wrong or missing parameter value for processisinerror");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("processendstate"))
                {
                    _logger.LogError("Wrong or missing parameter value for processendstate");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("lastchangeddatetime"))
                {
                    _logger.LogError("Wrong or missing parameter value for lastchangeddatetime");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("createddatetime"))
                {
                    _logger.LogError("Wrong or missing parameter value for createddatetime");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("visibledatetime"))
                {
                    _logger.LogError("Wrong or missing parameter value for visibledatetime");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("duedatetime"))
                {
                    _logger.LogError("Wrong or missing parameter value for duedatetime");
                    return false;
                }

            }
                
            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("continuationToken"))
                {
                    _logger.LogError("Wrong or missing parameter value for continuationToken");
                    return false;
                }
            }

            if (HasParameter("processisinerror"))
            {
                if (!HasParameterWithValue("size"))
                {
                    _logger.LogError("Wrong or missing parameter value for size");
                    return false;
                }
            }
            return true;
        }
    }
}
