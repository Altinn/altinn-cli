using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Services.Storage.CommandHandlers
{
    /// <summary>
    /// Commandhandler that is used to upload documents to the ALtinn Blob storage.  
    /// </summary>
    public class UploadDocument : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDocumentHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public UploadDocument(ILogger<UploadDocument> logger) : base(logger)
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
                return "UploadDocument";
            }
        }

        /// <summary>
        /// Gets the description of the command handler that is used by the Help function
        /// </summary>
        public string Description
        {
            get
            {
                return "Uploads and save documents from a specific folder to Storage";
            }
        }

        /// <summary>
        /// Gets how the command can be specified to get documents, is used by the Help function
        /// </summary>
        public string Usage
        {
            get
            {
                return $"Storage UploadDocument ownerid=<id> instanceid=<instanceid> file=<full path to file (includes filename)>-Upload ans save document to storage\n";
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


        public bool Run()
        {
            if (IsValid)
            {
                // values are validate both on exitent and value, så just fetch them
                string ownerId = CommandParameters.GetValueOrDefault("ownerid");
                string instanceId = CommandParameters.GetValueOrDefault("instanceid");
                string fullFileName = CommandParameters.GetValueOrDefault("file");

                string elementType = "default";

                FileStream stream = new FileStream(fullFileName, FileMode.Open);

                InstanceResponseMessage responsMessage = ClientWrapper.UploadDataElement(ownerId, instanceId, elementType, stream, fullFileName);


            }

            return true;
        }
            /// <summary>
            /// Verifies if the input parameters are valid.
            /// </summary>
            /// <returns></returns>
        protected bool Validate()
        {
            if (HasParameterWithValue("ownerid") && HasParameterWithValue("instanceid") && HasParameterWithValue("file"))
            {
                if (File.Exists(CommandParameters.GetValueOrDefault("file")))
                {
                    return true;
                }

               _logger.LogError("Upload file does not exists");
                return false;
            }

           _logger.LogError("Missing parameter values");
            return false;
        }
    }
}
