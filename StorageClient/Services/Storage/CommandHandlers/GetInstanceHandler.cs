using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StorageClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public class GetInstanceHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;

        public GetInstanceHandler(ILogger<GetInstanceHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                ClientWrapper = new StorageClientWrapper();
            }

        }

        public string Name
        {
            get
            {
                return "GetInstance";
            }
        }

        public string Description
        {
            get
            {
                return "Returns the metedata for an instance of an Application";
            }
        }

        public string Usage
        {
            get
            {
                return "AltinnCLI > storage GetInstance -instanceId=<instance-guid>";
            }
        }

        public string ServiceProvider
        {
            get
            {
                return "Storage";
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
            return Name;
        }

        public bool Run()
        {
            if (IsValid)
            {
                Stream response = ClientWrapper.GetInstances(int.Parse(CommandParameters.GetValueOrDefault("ownerid")),
                                         Guid.Parse(CommandParameters.GetValueOrDefault("instanceid")));


                if (HasParameterWithValue("savefile"))
                {
                    if (response != null)
                    {
                        string filefolder = (ApplicationManager.ApplicationConfiguration.GetSection("StorageOutputFolder").Get<string>());

                        // chekc if file folder exists, if not create it
                        if (!Directory.Exists(filefolder))
                        {
                            Directory.CreateDirectory(filefolder);

                        }
                        FileStream file = new FileStream(filefolder + "/" + CommandParameters.GetValueOrDefault("ownerid").ToString()+".json", FileMode.CreateNew);
                        response.Position = 0;
                        response.CopyTo(file);
                        file.Flush();
                        file.Close();
                    }
                }
                else
                {
                    StreamReader result = new StreamReader(response);
                    _logger.LogInformation(result.ReadToEnd());
                }    
            }
            else
            {
                _logger.LogError("Missing parameters. Required parameters for GetInstance is: OwnerId, InstanceId");
            }

            
            //documentStream = wrapper.GetDocument(instanceOwnerId, instanceGuid, dataId);
            return true;
        }

        private bool Validate()
        {
            return (HasParameterWithValue("ownerid") & HasParameterWithValue("instanceid"));
        }

    }
}
