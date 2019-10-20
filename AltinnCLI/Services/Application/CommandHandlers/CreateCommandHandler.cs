using AltinnCLI.Core;
using AltinnCLI.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Services.Application
{
    class CreateCommandHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper _clientWrapper = null;

        public CreateCommandHandler(ILogger<CreateCommandHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                _clientWrapper = new StorageClientWrapper();
            }
        }

        public string Name
        {
            get
            {
                return "Create";
            }
        }

        public string Description
        {
            get
            {
                return "Create a new application";
            }
        }

        public string Usage
        {
            get
            {
                return "AltinnCLI > Application create -content";
            }
        }

        public string ServiceProvider
        {
            get
            {
                return "Application";
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
            throw new NotImplementedException();
        }

        public bool Run()
        {
            if (IsValid)
            {
                string appid = CommandParameters["appid"];
                string instanceowner = CommandParameters["instanceowner"];
                string datamodel = CommandParameters["instanceowner"];

                string folder = CommandParameters["folder"];

                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
                if (Directory.Exists(folder))
                {
                    foreach (string filename in readFiles(folder))
                    {
                        string contentType = "application/octet-stream";

                        if (filename.Contains(".xml"))
                        {
                            contentType = "application/xml";
                        }
                        if (filename.Contains(".json"))
                        {
                            contentType = "application/json";
                        }

                        FileStream file = new FileStream(filename, FileMode.Open);
                        StringContent content = new StringContent(file.ToString());
                        multipartFormData.Add(content, contentType);
                    }

                    string response = _clientWrapper.CreateApplication(appid, instanceowner, multipartFormData);
                    _logger.LogInformation("App instanciated : ", response);
                }
                else
                {
                    _logger.LogError("Could not open folder");
                }

                

                
            }

            return true;
        }

        private String[] readFiles(string path)
        {
            if (Directory.Exists(path))
            { 
                return Directory.GetFiles(path);
            }
            else
            {
                return null;
            }
        }
        protected bool Validate()
        {
            bool valid = true;
            if (!HasParameterWithValue("appid"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter appId");
            }

            if (!HasParameterWithValue("instanceowner"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter instanceOwnerId");
            }

            if (!HasParameterWithValue("folder"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for folder");
            }

            if (!HasParameterWithValue("dataModel"))
            {
                if (HasParameterWithValue("folder"))
                {
                    _logger.LogError("No data model specified and no Default.xml file found in {0} ", CommandParameters["folder"]);
                }
                else
                {
                    _logger.LogError("No data model or data directory specified");
                }
                
                
            }

            return valid;
        }
    }
}
