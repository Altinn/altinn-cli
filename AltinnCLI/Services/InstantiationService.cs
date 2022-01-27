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
        private readonly XmlSerializer _serializer;
        public InstantiationService(InstantiationConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _serializer = new(typeof(ServiceOwner));
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

                ServiceOwner serviceOwner = _serializer.Deserialize(reader) as ServiceOwner;
                string externalShipmentReference = serviceOwner.Prefill.ExternalShipmentReference;

                (string failedShipmentsFile, ServiceOwner failedShipments) = SetUpErrorFile(inputFile, serviceOwner);

                foreach (ServiceOwnerPrefillReportee reportee in serviceOwner.Prefill.Reportee)
                {
                    string org = serviceOwner.ServiceOwnerName;

                    bool reporteeIsOrg = false;

                    if (Validator.IsValidOrganizationNumber(reportee.Id))
                    {
                        reporteeIsOrg = true;
                    }
                    else if (!Validator.IsValidPersonNumber(reportee.Id))
                    {
                        //throw new ArgumentException("Invalid reportee id provided", nameof(reportee.Id));
                        MoveReporteeToErrorFile(failedShipmentsFile, failedShipments, reportee);
                        continue;
                    }

                    foreach (ServiceOwnerPrefillReporteeFormTask formTask in reportee.FormTask)
                    {
                        ProcessFormTask(externalShipmentReference, reportee, reporteeIsOrg, formTask);
                    }
                }

                File.Move(inputFile, Path.Combine(_config.OutputFolder, new FileInfo(inputFile).Name));
            }

            return Task.CompletedTask;
        }

        private Task ProcessFormTask(string externalShipmentReference, ServiceOwnerPrefillReportee reportee, bool reporteeIsOrg, ServiceOwnerPrefillReporteeFormTask formTask)
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
                        { "ExternalShipmentReference", externalShipmentReference },
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

        private (string, ServiceOwner) SetUpErrorFile(string inputFile, ServiceOwner serviceOwner)
        {
            string failedShipmentsFile = Path.Combine(_config.ErrorFolder, $"failed_{new FileInfo(inputFile).Name}");
            ServiceOwner failedShipments = new ServiceOwner
            {
                ServiceOwnerName = serviceOwner.ServiceOwnerName,
                Subscription = serviceOwner.Subscription,
                Prefill = new ServiceOwnerPrefill
                {
                    ExternalShipmentReference = serviceOwner.Prefill.ExternalShipmentReference,
                    SequenceNo = serviceOwner.Prefill.SequenceNo,
                    Reportee = new()
                }
            };

            return (failedShipmentsFile, failedShipments);
        }

        private void MoveReporteeToErrorFile(string failedShipmentsFile, ServiceOwner failedShipments, ServiceOwnerPrefillReportee reportee)
        {
            failedShipments.Prefill.Reportee.Add(reportee);
            using var writer = XmlWriter.Create(failedShipmentsFile);

            _serializer.Serialize(writer, failedShipments);
        }
    }
}
