using AltinnCLI.Core;
using AltinnCLI.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
                string app = CommandParameters["app"];
                string org = CommandParameters["org"];
                string instanceowner = CommandParameters["instanceowner"];

                string folder = CommandParameters["folder"];

                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
                if (Directory.Exists(folder))
                {
                    foreach (string filePath in readFiles(folder))
                    {
                        string contentType = "application/octet-stream";

                        if (filePath.Contains(".xml"))
                        {
                            contentType = "application/xml";
                        }
                        if (filePath.Contains(".json"))
                        {
                            contentType = "application/json; charset=utf-8";
                        }

                        if (contentType != "application/octet-stream")
                        {
                            FileStream file = new FileStream(filePath, FileMode.Open);

                            StringContent content = new StringContent(file.ToString());
                            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                            content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data; name="+Path.GetFileNameWithoutExtension(filePath));
                            multipartFormData.Add(content, Path.GetFileName(filePath));
                        }
                    }

                    string response = _clientWrapper.CreateApplication(org, app, instanceowner, multipartFormData);
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
            if (!HasParameterWithValue("app"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter app");
            }

            if (!HasParameterWithValue("org"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter org");
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
