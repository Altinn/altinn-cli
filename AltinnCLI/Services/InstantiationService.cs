using Altinn.Platform.Storage.Interface.Models;

using AltinnCLI.Configurations;
using AltinnCLI.Helpers;
using AltinnCLI.Models;
using AltinnCLI.Services.Interfaces;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AltinnCLI.Services
{
    public class InstantiationService : IInstantiation
    {
        private readonly InstantiationConfig _config;
        private readonly ILogger _logger;

        public InstantiationService(InstantiationConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task Altinn2BatchInstantiation()
        {
            if (!Directory.Exists(_config.InputFolder))
            {
                throw new ArgumentException("Invalid input folder provided");
            }

            if (string.IsNullOrEmpty(_config.OutputFolder) || string.IsNullOrEmpty(_config.ErrorFolder))
            {
                throw new ArgumentException("Outpur and error folder must be provided");
            }

            Directory.CreateDirectory(_config.OutputFolder);
            Directory.CreateDirectory(_config.ErrorFolder);

            // error handling if values not configures.

            foreach (string inputFile in Directory.GetFiles(_config.InputFolder))
            {

                var reader = XmlReader.Create(inputFile);

                XmlSerializer serializer = new(typeof(ServiceOwner));

                ServiceOwner r = serializer.Deserialize(reader) as ServiceOwner;

                foreach (ServiceOwnerPrefillReportee reportee in r.Prefill.Reportee)
                {
                    string org = r.ServiceOwnerName;

                    bool reporteeIsOrg = false;

                    if (Validator.IsValidOrganizationNumber(reportee.Id))
                    {
                        reporteeIsOrg = true;
                    }
                    else if (!Validator.IsValidPersonNumber(reportee.Id))
                    {
                        throw new ArgumentException("Invalid reportee id provided", nameof(reportee.Id));
                    }

                    foreach (ServiceOwnerPrefillReporteeFormTask formTask in reportee.FormTask)
                    {
                        ProcessFormTask(reportee, reporteeIsOrg, formTask);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task ProcessFormTask(ServiceOwnerPrefillReportee reportee, bool reporteeIsOrg, ServiceOwnerPrefillReporteeFormTask formTask)
        {
            var appNameAvailable = _config.ApplicationIdLookup.TryGetValue(formTask.ExternalServiceCode, out string appId);

            if (appNameAvailable)
            {
                Instance i = new Instance
                {
                    AppId = appId,
                    InstanceOwner = new InstanceOwner(),
                    DataValues = new Dictionary<string, string>()
                    {
                        { "SendersReference", formTask.SendersReference },
                        { "ReceiversReference", formTask.ReceiversReference },
                        { "IdentifyingFields", string.Join(", ", formTask.IdentifyingFields.Select(f => f.Value).ToList())}
                    }
                };

                if (reporteeIsOrg)
                {
                    i.InstanceOwner.OrganisationNumber = reportee.Id;
                }
                else
                {
                    i.InstanceOwner.PersonNumber = reportee.Id;
                }

                var multipartFormData = BuildContentForInstance(i, formTask);

                IApplicationClientWrapper _clientWrapper = new ApplicationClientWrapper(_logger);

                string response = _clientWrapper.CreateInstance(appId.Split("/")[0], appId.Split("/")[1], string.Empty, multipartFormData);

                return Task.CompletedTask;
            }

            _logger.LogError("AppId not confiugred");
            // todo: error handling. remove form task from batch over to error
            return Task.CompletedTask;
        }

        private MultipartFormDataContent BuildContentForInstance(Instance instance, ServiceOwnerPrefillReporteeFormTask formTask)
        {
            MultipartContentBuilder contentBuilder = new(instance);

            foreach (ServiceOwnerPrefillReporteeFormTaskForm form in formTask.Form)
            {

                if (!_config.DataTypeLookup.ContainsKey(form.DataFormatId))
                {
                    _logger.LogError("Data type mapping missing for {DataFormatId}", form.DataFormatId);
                    break;
                }

                string formData = form.FormData;
                StringContent stringContent = new(formData);
                stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
                contentBuilder.AddDataElement(_config.DataTypeLookup.GetValueOrDefault(form.DataFormatId), stringContent);
            }

            return contentBuilder.Build();
        }
    }
}
