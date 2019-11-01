using Altinn.Platform.Storage.Models;
using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;

namespace AltinnCLI.Commands.Application
{

    /// <summary>
    /// Command for creating instances of an app. The command can be used both to create a single instance for 
    /// a specified instance owner, or for instanciating the app for several users simultaniously. The command
    /// also supports prefilling instances.
    /// </summary>
    class CreateInstanceSubCommandHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        /// <summary>
        /// Handles communication with the runtime API
        /// </summary>
        private IApplicationClientWrapper clientWrapper = null;

        /// <summary>
        /// Short code of the app the instance is being created for
        /// </summary>
        private string app = string.Empty;
        
        /// <summary>
        /// Short code for the application owner
        /// </summary>
        private string org = string.Empty;
        
        /// <summary>
        /// Id of the party the instance will be created for
        /// </summary>
        private string instanceOwnerId = string.Empty;
        
        /// <summary>
        /// Name of the file containing the datamodel for the instance. Defaults to 'Default.xml'
        /// </summary>
        private string dataModel = string.Empty;
        
        /// <summary>
        /// Name of the file containing the instance template. Defaults to 'Instance.json'
        /// </summary>
        private string instanceTemplate = string.Empty;
        
        /// <summary>
        /// Name of the folder where instance information is stored on the users local machine
        /// </summary>
        private string folder = string.Empty;
        
        /// <summary>
        /// Name of the folder holding prefill instance datamodels
        /// </summary>
        private string instanceData = string.Empty;

        /// <summary>
        /// Creates an instance of <see cref="CreateInstanceSubCommandHandler" /> class
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information</param>
        public CreateInstanceSubCommandHandler(ILogger<CreateInstanceSubCommandHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                clientWrapper = new ApplicationClientWrapper(_logger);
            }

            AddOptions();
        }

        public void AddOptions()
        {
            IOption app = (IOption)new Option<string>("app", Options["app"], "app");
            if (app.IsValid())
            {
                CliOptions.Add(app);
            }
        }

        /// <summary>
        /// Gets the name of the command
        /// </summary>
        public string Name
        {
            get
            {
                return "CreateInstance";
            }
        }

        /// <summary>
        /// Gets the description of the command. Will be used to generate help documentation
        /// </summary>
        public string Description
        {
            get
            {
                return "Create new instances of an application. Use the command to either create a single instance for a specific user, or create prefilled instances for multiple instance owners";
            }
        }

        /// <summary>
        /// Gets the usage of the command. Will be used to generate help documentation
        /// </summary>
        public string Usage
        {
            get
            {
                return $"AltinnCLI > Application createInstance app org instanceOwnerId dataModel instanceTemplate folder instanceData\n\n" +
                       $"\tapp <-a> \tApplication short code \n" +
                       $"\torg <-o> \tApplication owner identification code \n" +
                       $"\tinstanceOwnerId <-i> \tIdentification number of the person the app instance will be created for \n" +
                       $"\tdataModel <-dm> \tPath to xml document specifying the data model for the instance. If no value is given the tool will look for 'Default.xml'" +
                       $" in the specified input folder\n" +
                       $"\tinstanceTemplate <-t> \tApplication short code \n" +
                       $"\tfolder <-dir> \tPath to local folder containing data elements for the instance that is created\n" +
                       $"\tinstanceData <-data> \tPath to local folder containing prefill data for multiple  instance owners\n\n" +
                       $"Application createInstance app=<app code> org=<organisation code> instanceOwnerId=<id> folder=<path> " +
                       $"-Creates an instance of the app for an instance owner. Will scan folder for datamodel named Default.xml\n" +
                       $"Application createInstance app=<app code> org=<organisation code> instanceOwnerId=<id> dataModel=<filename> folder=<path> " +
                       $"-Creates an instance of the app for an instance owner based on the file specified by dataModel. Will scan folder for instance template and attachements\n";
            }
        }

        /// <summary>
        /// Gets the name of the CommandProvider for the command
        /// </summary>
        public string CommandProvider
        {
            get
            {
                return "Application";
            }
        }

        /// <summary>
        /// Gets the validation status of the command
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
        /// Processes and runs the command
        /// </summary>
        /// <returns>Returns true if the command completes succesfully</returns>
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
                    foreach (string filePath in readFiles(instanceData))
                    {
                        if (Path.GetExtension(filePath) == "xml")
                        {
                            string xmlFileName = Path.GetFileName(filePath);
                            string personNumber = Path.GetFileNameWithoutExtension(filePath);
                            multipartFormData = buildContentForMultipleInstances(xmlFileName);

                            try
                            {
                                string result = clientWrapper.CreateApplication(app, org, instanceOwnerId, multipartFormData);

                                Instance instanceResult = JsonConvert.DeserializeObject<Instance>(result);
                                File.WriteAllText($"{folder}\\{personNumber}.json", JsonConvert.SerializeObject(instanceResult, Formatting.Indented));

                            }
                            catch (Exception e)
                            {
                                File.WriteAllText($"{folder}\\error-{personNumber}.txt", $"{e.Message}");
                            }
                        }
                    }
                }

                // Handle single app instanciation
                multipartFormData = buildContentForInstance(folder);

                    string response = clientWrapper.CreateApplication(org, app, instanceOwnerId, multipartFormData);
                    _logger.LogInformation(response);
                }
                else
                {
                    _logger.LogError(@$"Could not open folder '{folder}'");
                }
            return true;
        }

        private MultipartFormDataContent buildContentForInstance(string path)
        {
            MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
            if (Directory.Exists(path))
            {
                foreach (string filePath in readFiles(path))
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

                return multipartFormData;
            }
            else
            {
                _logger.LogError(@$"Could not open folder '{folder}'");
                return null;
            }
        }

        private MultipartFormDataContent buildContentForMultipleInstances(string path)
        {
            string xmlFileName, personNumber;

            if (Path.GetExtension(path) == "xml")
            {
                xmlFileName = Path.GetFileName(path);

                // The person number is the XML filename
                personNumber = Path.GetFileNameWithoutExtension(path);

                Instance instanceTemplate = new Instance()
                {
                    InstanceOwnerLookup = new InstanceOwnerLookup()
                    {
                        PersonNumber = personNumber,
                    }
                };

                MultipartFormDataContent content = new MultipartContentBuilder(instanceTemplate)
                .AddDataElement("default", new FileStream(path, FileMode.Open), "application/xml")
                .Build();

                return content;

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Reads all files in the directory specified by <see cref="path" />
        /// </summary>
        /// <param name="path">Path to the directory that will be read</param>
        /// <returns>A list of strings representing the path to each file in the directory</returns>
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
        /// Reads the value of a parameter if it has been set
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <returns>Returns the value of the parameter if one is given, null otherwise</returns>
        private string getParamValue(string paramName)
        {
            if (HasParameter(paramName))
            {
                return DictOptions[paramName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Verifies that the input parameters are valid.
        /// </summary>
        /// <returns>True if the command is valid, false if any required parameters are missing</returns>
        protected bool Validate()
        {
            bool valid = true;
            if (!HasParameterWithValue("app"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter: 'app'");
            }

            if (!HasParameterWithValue("org"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter: 'org'");
            }

            if (!HasParameterWithValue("instanceowner"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter: 'instanceOwnerId'");
            }

            if (!HasParameterWithValue("folder"))
            {
                valid = false;
                _logger.LogError("Invalid or missing value for parameter: 'folder'");
            }

            if (!HasParameterWithValue("dataModel"))
            {
                if (HasParameterWithValue("folder"))
                {
                    valid = false;
                    try
                    {
                        string[] files = readFiles(DictOptions["folder"]);
                        foreach(string file in files)
                        {
                            if (file.Contains("default.xml"))
                            {
                                valid = true;
                            }
                        }

                        if (!valid)
                        {
                            _logger.LogError($@"No data model specified and no Default.xml file found in {DictOptions["folder"]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($@"Error opening '{DictOptions["folder"]}': {ex.Message}");
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

public class MultipartContentBuilder
{
    private MultipartFormDataContent builder;

    public MultipartContentBuilder(Instance instanceTemplate)
    {
        builder = new MultipartFormDataContent();
        if (instanceTemplate != null)
        {
            StringContent instanceContent = new StringContent(JsonConvert.SerializeObject(instanceTemplate), Encoding.UTF8, "application/json");

            builder.Add(instanceContent, "instance");
        }
    }

    public MultipartContentBuilder AddDataElement(string elementType, Stream stream, string contentType)
    {
        StreamContent streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        builder.Add(streamContent, elementType);

        return this;
    }

    public MultipartContentBuilder AddDataElement(string elementType, StringContent content)
    {
        builder.Add(content, elementType);

        return this;
    }

    public MultipartFormDataContent Build()
    {
        return builder;
    }

}
