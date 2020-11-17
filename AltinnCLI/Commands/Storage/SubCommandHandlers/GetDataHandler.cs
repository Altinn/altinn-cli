using System;
using System.IO;

using Altinn.Platform.Storage.Interface.Models;
using AltinnCLI.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AltinnCLI.Commands.Storage.SubCommandHandlers
{
    /// <summary>
    /// Command handler that is used to fetch documents from Altinn Blob storage.  
    /// </summary>
    public class GetDataHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        private readonly IStorageClientWrapper _clientWrapper;

        private readonly string _updateInstances = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDataHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information</param>
        public GetDataHandler(ILogger<GetDataHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                _clientWrapper = new StorageClientWrapper(_logger);
            }
            else
            {
                _clientWrapper = new StorageClientFileWrapper(_logger);
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
                string usage = $"\n" +
                $"Storage GetData org=<appid> processIsComplete=<true/false> lastChangedDate=<gt:2019-10-23>  -Fetch documents for org that is completed since lastchangeddate \n" +
                $"Storage GetData appid=<appid> processIsComplete=<true/false> lastChangedDate=<gt:2019-10-23>  -Fetch documents for app that is completed since lastchangeddate \n" +
                $"Storage GetData ownerid=<id>  -Fetch all documents for owner \n" +
                $"Storage GetData ownerid=<id> instanceId=<instance-guid> dataId=<data-guid> -Fetch specific data element \n" +
                $"\n" +
                $" Available parameters for the command that download documents for an org or app \n";

                foreach (IOption opt in SelectableCliOptions)
                {
                    usage += $"\t{opt.Name}\t\t {opt.Description} \n";
                }

                return usage;
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
                int? ownerId = (int?)GetOptionValue("ownerId");
                Guid? instanceId = (Guid?)GetOptionValue("instanceId");
                Guid? dataId = (Guid?)GetOptionValue("dataId");

                if ((ownerId.HasValue) && (instanceId != null) && (dataId != null))
                {
                    Stream stream = _clientWrapper.GetData((int)ownerId, (Guid)instanceId, (Guid)dataId);

                    if (stream != null)
                    {
                        string fileFolder = $@"{ownerId}\{instanceId}";

                        CliFileWrapper.SaveToFile(fileFolder, ((Guid)dataId).ToString(), stream);
                    }
                }
                else
                {
                    GetDocumentFromInstances();
                }
            }
            else
            {
                ErrorMessage = "Error in command parameter(s)\n";
                _logger.LogInformation(ErrorMessage);
            }

            return true;
        }
 
        /// <summary>
        /// Fetch instance data and call member method to fetch and save document on file
        /// </summary>
        private void GetDocumentFromInstances()
        {
            InstanceResponseMessage responsMessage = null;
            string appId = (string)GetOptionValue("appId");
            string org = (string)GetOptionValue("org");
            int? ownerId = (int?)GetOptionValue("ownerId"); 
            Guid? instanceId = (Guid?)GetOptionValue("instanceId");

            if (!string.IsNullOrEmpty(appId) || !string.IsNullOrEmpty(org))
            {
                responsMessage = _clientWrapper.GetInstanceMetaData(this.SelectableCliOptions);

            }
            else if (ownerId != null)
            {
                responsMessage = _clientWrapper.GetInstanceMetaData(ownerId, instanceId);
            }
            else
            {
                _logger.LogError($"No valid combination of command options, please use help to find available combintations");
                return;
            }

            if (responsMessage != null)
            {
                _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

                Instance[] instances = responsMessage.Instances;
                FetchAndSaveData(instances, responsMessage.Next, _updateInstances);
            }
            else
            {
                _logger.LogInformation($"No data available for instance");
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
                int numberOfFiles = 0;
                foreach (DataElement data in instance.Data)
                { 
                    string url = data.SelfLinks.Platform;
                    Stream responseData = _clientWrapper.GetData(url, data.ContentType);

                    if (responseData != null)
                    {
                        string instanceGuidId = instance.Id.Split('/')[1];
                        string fileName = $"{data.DataType.ToString()}_{((string.IsNullOrEmpty(data.Filename)) ? data.Id : data.Filename)}";

                        string fileFolder = $@"{instance.InstanceOwner.PartyId}\{instanceGuidId}";

                        if ( CliFileWrapper.SaveToFile(fileFolder, fileName, responseData) )
                        {
                            _logger.LogInformation($"File:{fileName} saved at {fileFolder}");
                        }
                        numberOfFiles++;
                    }
                }

                if(numberOfFiles == 0)
                {
                    _logger.LogInformation($"No files received for instance:{instance.Id}");
                }
            }

            if (nextLink != null)
            {
                InstanceResponseMessage responsMessage = _clientWrapper.GetInstanceMetaData(nextLink);
                _logger.LogInformation($"Fetched {responsMessage.Instances.Length} instances. Count={responsMessage.Count}");

                FetchAndSaveData(responsMessage.Instances, responsMessage.Next, updateInstances);
            }
        }
    }
}
