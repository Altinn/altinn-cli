using Altinn.Platform.Storage.Models;
using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AltinnCLI.Commands.Storage
{
    /// <summary>
    /// Commandhandler that is used to fetch documents from Altinn Blob storage.  
    /// </summary>
    public class GetDataHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
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
        public GetDataHandler(ILogger<GetDataHandler> logger) : base(logger)
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
                return "GetData";
            }
        }

        /// <summary>
        /// Gets the description of the command handler that is used by the Help function
        /// </summary>
        public string Description
        {
            get
            {
                return "Download data from Storage and saves to folder set by config value StorageOutputFolder";
            }
        }

        /// <summary>
        /// Gets how the command can be specified to get documents, is used by the Help function
        /// </summary>
        public string Usage
        {
            get
            {
                return  $"\n"+
                        $"Storage GetData org=<appid> processIsComplete=<true/fals> lastChangedDate=<gt:2019-10-23>  -Fetch documents for org that is completed since lastchangeddate \n" +
                        $"Storage GetData appid=<appid> processIsComplete=<true/fals> lastChangedDate=<gt:2019-10-23>  -Fetch documents for app that is completed since lastchangeddate \n" +
                        $"\n" +
                        $" Available parameters for the command that download documents for an org or app \n" + 
                        $"    org                 -org \n" +
                        $"    appId               -application id\n" +
                        $"    currentTaskId       -running process current task id\n" +
                        $"    processIsComplete   -is process complete\n" +
                        $"    processIsInError    -is process in error\n" +
                        $"    processEndState     -process end state\n" +
                        $"    labels              -labels\n" +
                        $"    lastChangedDateTime -last changed date \n" +
                        $"    createdDateTime     -created time \n"+
                        $"    visibleDateTime     -the visible date time \n" +
                        $"    dueDateTime         -the due date time \n" +
                        $"    continuationToken   -continuation token \n" +
                        $"    size                -the page size \n\n" +
                        $"Storage GetData ownerid=<id>  -Fetch all documents for owner \n" +
                        $"Storage GetData ownerid=<id> instanceId=<instance-guid> dataId=<data-guid> -Fetch specific data element \n";
            }
        }

        /// <summary>
        /// Gets the name of the CommandProvider that uses this command
        /// </summary>
        public string CommandProvider
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
                    Stream stream = ClientWrapper.GetData((int)ownerId, (Guid)instanceId, (Guid)dataId);

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
            ownerId = Options.GetValueOrDefault("ownerid") != null ? int.Parse(Options.GetValueOrDefault("ownerid")) : (int?)null;
            instanceId = Options.GetValueOrDefault("instanceid") != null ? Guid.Parse(Options.GetValueOrDefault("instanceid")) : (Guid?)null;
            dataId = Options.GetValueOrDefault("dataid") != null ? Guid.Parse(Options.GetValueOrDefault("dataid")) : (Guid?)null;
            updateInstances = Options.GetValueOrDefault("updateinstances") != null ? Options.GetValueOrDefault("instanceid") : string.Empty;

            appId = Options.GetValueOrDefault("appid") != null ? Options.GetValueOrDefault("appid") : string.Empty;
            org = Options.GetValueOrDefault("org") != null ? Options.GetValueOrDefault("org") : string.Empty;

            processIsComplete = Options.GetValueOrDefault("processsscomplete") != null ? bool.Parse(Options.GetValueOrDefault("processIsComplete")) : false;
            processIsInError = Options.GetValueOrDefault("processisisError") != null ? bool.Parse(Options.GetValueOrDefault("processisinerror")) : false;
            processEndState = Options.GetValueOrDefault("processendstate") != null ? Options.GetValueOrDefault("processendstate") : string.Empty;

            lastChangedDateTime = Options.GetValueOrDefault("lastchangedatetime") != null ? Options.GetValueOrDefault("lastchangeddatetime") : string.Empty;
            createdDateTime = Options.GetValueOrDefault("createddatetime") != null ? Options.GetValueOrDefault("createdtatetime") : string.Empty;
            visibleDateTime = Options.GetValueOrDefault("visibledatetime") != null ? Options.GetValueOrDefault("visibledatetime") : string.Empty;
            dueDateTime = Options.GetValueOrDefault("duedatetime") != null ? Options.GetValueOrDefault("duedatetme") : string.Empty;

            continuationToken = Options.GetValueOrDefault("continuationToken") != null ? Options.GetValueOrDefault("continuationToken") : string.Empty;
            size = Options.GetValueOrDefault("size") != null ? int.Parse(Options.GetValueOrDefault("size")) : (int?)null;
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
                responsMessage = ClientWrapper.GetInstanceMetaData(Options);

            }
            else if (ownerId != null)
            {
                responsMessage = ClientWrapper.GetInstanceMetaData(ownerId, instanceId);
            }

            if (responsMessage != null)
            {
                _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

                Instance[] instances = responsMessage.Instances;
                FetchAndSaveData(instances, responsMessage.Next, updateInstances);
            }

        }


        /// <summary>
        ///  Fetch document from storage and save it to file. If the Next link is defined continue to read rest of instances and data
        /// </summary>
        /// <param name="instances">The instances for which the data shall be fetched</param>
        /// <param name="nextLink">The fetch of data elements is paged, the next link shall be used to fetch next page of instances</param>
        /// <param name="updateInstances"></param>
        private void FetchAndSaveData(Instance[] instances, Uri nextLink, string updateInstances)
        {
            foreach (Instance instance in instances)
            {
                foreach (DataElement data in instance.Data)
                { 
                    string url = data.DataLinks.Platform;
                    Stream responsData = ClientWrapper.GetData(url, data.ContentType);

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

                FetchAndSaveData(responsMessage.Instances, responsMessage.Next, updateInstances);
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
                _logger.LogError("Only org or app can be defined as parameter");
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
