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
        private readonly ILogger _logger;

        public CreateCommandHandler(ILogger<CreateCommandHandler> logger)
        {
            _logger = logger;

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

                StreamReader streamReader = new StreamReader(CommandParameters["content"]);

                string filecontent = streamReader.ReadToEnd();
                StringContent content = new StringContent(filecontent);

                //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
                //content.Headers.Add("boundary", "abcdefg");

                string response = _clientWrapper.CreateApplication(appid, instanceowner, content);

                _logger.LogInformation("App instanciated : ", response);
            }
            else
            {
                _logger.LogInformation("Missing parameters");
            }
            return true;
        }

        protected bool Validate()
        {
            return (HasParameterWithValue("appid") & HasParameterWithValue("instanceowner") & HasParameterWithValue("content"));
        }
    }
}
