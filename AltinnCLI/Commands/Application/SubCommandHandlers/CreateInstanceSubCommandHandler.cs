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
using System.Linq;

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
        /// Creates an instance of <see cref="CreateInstanceSubCommandHandler" /> class
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information</param>
        public CreateInstanceSubCommandHandler(ILogger<CreateInstanceSubCommandHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                clientWrapper = new ApplicationClientWrapper(_logger);
            }
            else
            {
                clientWrapper = new ApplicationFileClientWrapper(_logger);
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
                string instanceTemplate = (string)GetOptionValue("instancetemplate");
                string folder = (string)GetOptionValue("folder");
                string app = (string)GetOptionValue("app");
                string org = (string)GetOptionValue("org");
                string instanceOwnerId = (string)GetOptionValue("ownerid");
                string instanceData = (string)GetOptionValue("instanceData");

                MultipartFormDataContent multipartFormData;

                // Handle Prefill data
                if (!String.IsNullOrEmpty(instanceTemplate))
                {
                    foreach (string filePath in ReadFiles(instanceData))
                    {
                        if (Path.GetExtension(filePath) == "xml")
                        {
                            string xmlFileName = Path.GetFileName(filePath);
                            string personNumber = Path.GetFileNameWithoutExtension(filePath);
                            multipartFormData = buildContentForMultipleInstances(xmlFileName);

                            try
                            {
                                string result = clientWrapper.CreateInstance(app, org, instanceOwnerId, multipartFormData);

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
                else
                {
                    multipartFormData = BuildContentForInstance(folder);

                    string response = clientWrapper.CreateInstance(org, app, instanceOwnerId, multipartFormData);
                    _logger.LogInformation(response);
                }
                
            }

            return true;
        }

        private MultipartFormDataContent BuildContentForInstance(string path)
        {
            if (Directory.Exists(path))
            {
                FileStream stream = null;
                Instance instance = new Instance();
                string formData = string.Empty;

                
                foreach (string filePath in ReadFiles(path))
                {
                    if (filePath.Contains(".xml"))
                    {

                        formData = filePath;
                    }

                    if (filePath.Contains(".json"))
                    {
                        instance = JsonConvert.DeserializeObject<Instance>(File.ReadAllText(filePath));
                    }
                }

                if (instance.AppId != null && !string.IsNullOrEmpty(formData))
                {
                    StringContent stringContent = new StringContent(File.ReadAllText(formData));
                    stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
                    MultipartFormDataContent content = new MultipartContentBuilder(instance)
                     .AddDataElement("default", stringContent)
                     .Build();

                    return content;
                } 
                else
                {
                    _logger.LogError($@"{path} is must contain both an instance template and form data");
                    return null;
                }
            }
            else
            {
                _logger.LogError(@$"Could not open folder '{path}'");
                return null;
            }
        }

        private MultipartFormDataContent buildContentForMultipleInstances(string path)
        {
            string personNumber;

            if (Path.GetExtension(path) == "xml")
            {
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
        private String[] ReadFiles(string path)
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
        /// Verifies that the input parameters are valid.
        /// </summary>
        /// <returns>True if the command is valid, false if any required parameters are missing</returns>
        public override bool Validate()
        {
            bool valid = true;

            if (!HasParameterWithValue("app") || !HasParameterWithValue("org"))
            {
                valid = false;
            }

            return valid;
        }
    }
}
