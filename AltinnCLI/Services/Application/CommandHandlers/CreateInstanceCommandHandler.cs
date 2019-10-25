using AltinnCLI.Core;
using AltinnCLI.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AltinnCLI.Services.Application
{

    /// <summary>
    /// 
    /// </summary>
    class CreateInstanceCommandHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        /// <summary>
        /// 
        /// </summary>
        private IStorageClientWrapper _clientWrapper = null;

        /// <summary>
        /// 
        /// </summary>
        private string app = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        private string org = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        private string instanceOwnerId = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        private string dataModel = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        private string instanceTemplate = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        private string folder = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        private string instanceData = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public CreateInstanceCommandHandler(ILogger<CreateInstanceCommandHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                _clientWrapper = new StorageClientWrapper(_logger);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return "CreateInstance";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                return "Create new instances of an application. Use the command to either create a single instance for a specific user, or create prefilled instances for multiple instance owners";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Usage
        {
            get
            {
                return $"AltinnCLI > Application createInstance -app -org -instanceOwnerId -dataModel -instanceTemplate -folder -instanceData\n\n" +
                       $"\t-app <-a> \tApplication short code \n" +
                       $"\t-org <-o> \tApplication owner identification code \n" +
                       $"\t-instanceOwnerId <-i> \tIdentification number of the person the app instance will be created for \n" +
                       $"\t-dataModel <-dm> \tPath to xml document specifying the data model for the instance. If no value is given the tool will look for 'Default.xml' in the specified input folder\n" +
                       $"\t-instanceTemplate <-t> \tApplication short code \n" +
                       $"\t-folder <-dir> \tPath to local folder containing data elements for the instance that is created\n" +
                       $"\t-instanceData <-data> \tPath to local folder containing prefill data for multiple  instance owners\n";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceProvider
        {
            get
            {
                return "Application";
            }
        }

        /// <summary>
        /// 
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (IsValid)
            {
                app = getParamValue("app");
                org = getParamValue("org");
                instanceOwnerId = getParamValue("instanceOwnerId");
                dataModel = getParamValue("dataModel");
                instanceTemplate = getParamValue("instanceTemplate");
                folder = getParamValue("folder");
                instanceData = getParamValue("instanceData");

                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();

                // Handle Prefill data
                if (!String.IsNullOrEmpty(instanceTemplate))
                {

                }

                // Handle single app instanciation
                

                if (Directory.Exists(folder))
                {
                    foreach (string filePath in readFiles(folder))
                    {
                        string contentType = "application/octet-stream";
                        string contentName = "default";

                        if (filePath.Contains(".xml"))
                        {
                            contentType = "application/xml";
                        }
                        if (filePath.Contains(".json"))
                        {
                            contentType = "application/json; charset=utf-8";
                            contentName = "instance";
                        }

                        if (contentType != "application/octet-stream")
                        {
                            StringContent content = new StringContent(File.ReadAllText(filePath));
                            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                            content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse($"form-data; name={contentName}");
                            multipartFormData.Add(content, Path.GetFileNameWithoutExtension(filePath));
                        }
                    }

                    string response = _clientWrapper.CreateApplication(org, app, instanceOwnerId, multipartFormData);
                    _logger.LogInformation(response);
                }
                else
                {
                    _logger.LogError(@$"Could not open folder '{folder}'");
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private string getParamValue(string paramName)
        {
            if (HasParameter(paramName))
            {
                return CommandParameters[paramName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                    valid = false;
                    try
                    {
                        string[] files = readFiles(CommandParameters["folder"]);
                        foreach(string file in files)
                        {
                            if (file.Contains("default.xml"))
                            {
                                valid = true;
                            }
                        }

                        if (!valid)
                        {
                            _logger.LogError($@"No data model specified and no Default.xml file found in {CommandParameters["folder"]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($@"Error opening '{CommandParameters["folder"]}': {ex.Message}");
                    }
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
