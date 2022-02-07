using System;
using System.Collections.Generic;
using System.IO;

using AltinnCLI.Clients;
using AltinnCLI.Commands.Core;
using AltinnCLI.Services;
using AltinnCLI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AltinnCLI.Commands.Storage
{
    /// <summary>
    /// Command for getting metadata for an instances of an app.
    /// </summary>
    public class GetInstanceHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        private readonly InstanceClient _client;

        /// <summary>
        /// Creates an instance of the <see cref="GetInstanceHandler" /> class
        /// </summary>
        /// <param name="logger"></param>
        public GetInstanceHandler(InstanceClient client, ILogger<GetInstanceHandler> logger) : base(logger)
        {

            _client = client;

        }

        /// <summary>
        /// Gets the name of the command
        /// </summary>
        public string Name
        {
            get
            {
                return "GetInstance";
            }
        }

        /// <summary>
        /// Gets the description of the command. Will be used to generate help documentation
        /// </summary>
        public string Description
        {
            get
            {
                return "Returns the metedata for an instance of an Application";
            }
        }

        /// <summary>
        /// Gets the usage of the command. Will be used to generate help documentation
        /// </summary>
        public string Usage
        {
            get
            {
                return "AltinnCLI > storage GetInstance instanceOwnerId=<id> instanceId=<instance-guid> saveToFile";
            }
        }

        /// <summary>
        /// Gets the name of the CommandProvider for the command
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

        /// <summary>
        /// Processes and runs the command
        /// </summary>
        /// <returns>Returns true if the command completes succesfully</returns>
        public bool Run()
        {
            if (IsValid)
            {
                Stream response = _client.GetInstanceStream(int.Parse(DictOptions.GetValueOrDefault("ownerid")),
                                         Guid.Parse(DictOptions.GetValueOrDefault("instanceid"))).Result;


                if (HasParameter("savetofile"))
                {
                    string fileName = DictOptions.GetValueOrDefault("instanceid").ToString() + ".json";
                    if (response != null)
                    {
                        string fileFolder = DictOptions.GetValueOrDefault("ownerid");
      

                        CliFileWrapper.SaveToFile(fileFolder, fileName, response);
                        
                    }
                }
                else
                {
                    StreamReader result = new(response);
                    _logger.LogInformation(result.ReadToEnd());
                }    
            }
            else
            {
                _logger.LogError("Missing parameters. Required parameters for GetInstance is: OwnerId, InstanceId");
            }

            
            //documentStream = wrapper.GetData(instanceOwnerId, instanceGuid, dataId);
            return true;
        }

        /// <summary>
        /// Verifies that the input parameters are valid.
        /// </summary>
        /// <returns>True if the command is valid, false if any required parameters are missing</returns>
        public override bool Validate()
        {
            return (HasParameterWithValue("ownerid") & HasParameterWithValue("instanceid"));
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

    }
}
