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
                reader.Close();

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
                        _logger.LogError("Invalid reportee id provided {ReporteeId}. Mocving to error file", reportee.Id);
                        MoveReporteeToErrorFile(failedShipmentsFile, failedShipments, reportee);
                        continue;
                    }

                    foreach (ServiceOwnerPrefillReporteeFormTask formTask in reportee.FormTask)
                    {
                        ProcessFormTask(externalShipmentReference, reportee, reporteeIsOrg, formTask, failedShipmentsFile, failedShipments);
                    }
                }

                File.Move(inputFile, Path.Combine(_config.OutputFolder, new FileInfo(inputFile).Name), true);
            }

            return Task.CompletedTask;
        }

        private Task ProcessFormTask(
            string externalShipmentReference,
            ServiceOwnerPrefillReportee reportee,
            bool reporteeIsOrg,
            ServiceOwnerPrefillReporteeFormTask formTask,
            string failedShipmentsFile,
            ServiceOwner failedShipments)
        {
            var appNameAvailable = _config.ApplicationIdLookup.TryGetValue(formTask.ExternalServiceCode, out string appId);

            if (!appNameAvailable)
            {
                _logger.LogError($"AppId not confiugred for external service code {formTask.ExternalServiceCode}. Moving form task to error file.");
                MoveFormTaskToErrorFile(failedShipmentsFile, failedShipments, reportee, formTask);
                return Task.CompletedTask;
            }

            Instance i = new()
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

            try
            {
                var multipartFormData = BuildContentForInstance(i, formTask);
                IApplicationClientWrapper _clientWrapper = new ApplicationClientWrapper(_logger);
                string response = _clientWrapper.CreateInstance(appId.Split("/")[0], appId.Split("/")[1], string.Empty, multipartFormData);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MoveFormTaskToErrorFile(failedShipmentsFile, failedShipments, reportee, formTask);
                _logger.LogError("Generating data elements for instance failed with exception. Moving form task to error file.", ex);

            }
            catch (HttpRequestException ex)
            {
                MoveFormTaskToErrorFile(failedShipmentsFile, failedShipments, reportee, formTask);
                _logger.LogError("Instance creation failed with exception. Moving form task to error file.", ex);
            }
            catch (Exception ex)
            {
                MoveFormTaskToErrorFile(failedShipmentsFile, failedShipments, reportee, formTask);
                _logger.LogError("An unknown exception occured. Moving form task to error file.", ex);
            }

            return Task.CompletedTask;
        }

        private MultipartFormDataContent BuildContentForInstance(Instance instance, ServiceOwnerPrefillReporteeFormTask formTask)
        {
            MultipartContentBuilder contentBuilder = new(instance);

            foreach (ServiceOwnerPrefillReporteeFormTaskForm form in formTask.Form)
            {
                var validDataFormatId = _config.DataTypeLookup.TryGetValue(form.DataFormatId, out string dataType);

                if (!validDataFormatId)
                {
                    _logger.LogError("Data type mapping missing for {DataFormatId}", form.DataFormatId);
                    throw new ArgumentOutOfRangeException(nameof(formTask), "Data type mapping missing for { DataFormatId}. Moving formTask to error file.", form.DataFormatId);
                }

                string formData = form.FormData;
                StringContent stringContent = new(formData);
                stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
                contentBuilder.AddDataElement(dataType, stringContent);
            }

            return contentBuilder.Build();
        }

        private (string, ServiceOwner) SetUpErrorFile(string inputFile, ServiceOwner serviceOwner)
        {
            string failedShipmentsFile = Path.Combine(_config.ErrorFolder, $"failed_{new FileInfo(inputFile).Name}");

            ServiceOwner failedShipments = new()
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

            using StreamWriter sw = new(failedShipmentsFile);
            _serializer.Serialize(sw, failedShipments);
            sw.Close();
        }

        private void MoveFormTaskToErrorFile(
            string failedShipmentsFile,
            ServiceOwner failedShipments,
            ServiceOwnerPrefillReportee reportee,
            ServiceOwnerPrefillReporteeFormTask formTask)
        {
            ServiceOwnerPrefillReportee failedReportee = failedShipments.Prefill.Reportee.FirstOrDefault(r => r.Id == reportee.Id);
            if (failedReportee == null)
            {
                failedReportee = new ServiceOwnerPrefillReportee
                {
                    Id = reportee.Id,
                    Field = reportee.Field,
                    FormTask = new()
                };
            }

            failedReportee.FormTask.Add(formTask);
            failedShipments.Prefill.Reportee.Add(failedReportee);

            using StreamWriter sw = new(failedShipmentsFile);
            _serializer.Serialize(sw, failedShipments);
            sw.Close();
        }
    }
}
