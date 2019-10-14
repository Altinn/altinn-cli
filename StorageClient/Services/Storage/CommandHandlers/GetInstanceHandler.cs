using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services.Storage
{
    public class GetInstanceHandler : CommandHandlerBase, ICommandHandler, IHelp
    {
        private IStorageClientWrapper ClientWrapper = null;
        private readonly ILogger _logger;

        public GetInstanceHandler(ILogger<GetInstanceHandler> logger)
        {
            _logger = logger;

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
                ClientWrapper.GetInstance(int.Parse(CommandParameters.GetValueOrDefault("OwnerId")),
                                         Guid.Parse(CommandParameters.GetValueOrDefault("InstanceId")));
            }

            //documentStream = wrapper.GetDocument(instanceOwnerId, instanceGuid, dataId);
            return true;
        }

        private bool Validate()
        {
            return (HasParameterWithValue("OwnerId") & HasParameterWithValue("InstanceId"));
        }
    }
}
