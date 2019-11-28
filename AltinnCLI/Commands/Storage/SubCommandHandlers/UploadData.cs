using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AltinnCLI.Commands.Storage
{
    /// <summary>
    /// Commandhandler that is used to upload data to the Altinn Blob storage.  
    /// </summary>
    public class UploadData : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDocumentHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public UploadData(ILogger<UploadData> logger) : base(logger)
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
                return "UploadData";
            }
        }

        /// <summary>
        /// Gets the description of the command handler that is used by the Help function
        /// </summary>
        public string Description
        {
            get
            {
                return "Uploads and save files from a specific folder to Storage";
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
                $"Storage UploadData ownerId=<ownerid> instanceId=<instanceguid> elementType=<elementtype> file=c:<filename with full path>\n" +
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


        public List<IOption> Options { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return Name;
        }


        public bool Run()
        {
            if (IsValid)
            {
                IOption fileNameOption = SelectableCliOptions.FirstOrDefault(x => string.Equals(x.Name, "file", StringComparison.OrdinalIgnoreCase));

                MemoryStream memoryStream = CliFileWrapper.GetFile(fileNameOption.Value);

                InstanceResponseMessage responsMessage = ClientWrapper.UploadDataElement(SelectableCliOptions, memoryStream, fileNameOption.Value);
            }

            return true;
        }

        public override bool Validate()
        {
            // validate options, UploadData requires that ownerid, instanceGuid, dataType and file name is specified and valid
            // the values for the properties set is validate and found valid so this validation verifies that all the required is set

            if (GetOption("ownerid").IsAssigned && GetOption("instanceId").IsAssigned && 
                GetOption("elementType").IsAssigned && GetOption("file").IsAssigned)
            {
                return true;
            }

            _logger.LogError("No valid combination of options, see Help for correct and required option");
            _logger.LogInformation(GetParameterErrors());

            return false;
        }

    }
}
