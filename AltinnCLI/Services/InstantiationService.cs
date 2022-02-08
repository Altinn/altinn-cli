using Altinn.Platform.Storage.Interface.Models;

using AltinnCLI.Clients;
using AltinnCLI.Configurations;
using AltinnCLI.Helpers;
using AltinnCLI.Models;
using AltinnCLI.Services.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;

namespace AltinnCLI.Services
{
    public class InstantiationService : IInstantiation
    {
        private readonly InstantiationConfig _config;
        private readonly ILogger<IInstantiation> _logger;
        private readonly XmlSerializer _serializer;
        private readonly InstanceClient _client;

        public InstantiationService(IOptions<InstantiationConfig> config, InstanceClient client, ILogger<IInstantiation> logger)
        {
            _config = config.Value;
            _logger = logger;
            _serializer = new(typeof(ServiceOwner));
            _client = client;
        }

        public bool Altinn2BatchInstantiation()
        {
            if (!Directory.Exists(_config.InputFolder))
            {
                throw new ArgumentException("Invalid input folder provided");
            }

            if (string.IsNullOrEmpty(_config.OutputFolder) || string.IsNullOrEmpty(_config.ErrorFolder))
            {
                throw new ArgumentException("Output and error folder must be provided");
            }

            Directory.CreateDirectory(_config.OutputFolder);
            Directory.CreateDirectory(_config.ErrorFolder);

            foreach (string inputFile in Directory.GetFiles(_config.InputFolder))
            {
                var reader = XmlReader.Create(inputFile);
                ServiceOwner serviceOwner = _serializer.Deserialize(reader) as ServiceOwner;
                reader.Close();

                string externalShipmentReference = serviceOwner.Prefill.ExternalShipmentReference;

                string sentItemsFileName = $"{externalShipmentReference}.json";
                SentItems sentItems = GetSentItems(sentItemsFileName);

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
                        if (!sentItems.Reference.Contains(formTask.SendersReference))
                        {
                            ProcessFormTask(externalShipmentReference, reportee, reporteeIsOrg, formTask, failedShipmentsFile, failedShipments, sentItemsFileName, sentItems);
                        }
                    }
                }

                File.Move(inputFile, Path.Combine(_config.OutputFolder, new FileInfo(inputFile).Name), true);
                File.Delete(Path.Combine(_config.OutputFolder, sentItemsFileName));
            }

            return true;
        }

        private bool ProcessFormTask(
            string externalShipmentReference,
            ServiceOwnerPrefillReportee reportee,
            bool reporteeIsOrg,
            ServiceOwnerPrefillReporteeFormTask formTask,
            string failedShipmentsFile,
            ServiceOwner failedShipments,
            string sentItemsFileName,
            SentItems sentItems)
        {
            var appNameAvailable = _config.ApplicationIdLookup.TryGetValue(formTask.ExternalServiceCode, out string appId);

            if (!appNameAvailable)
            {
                _logger.LogError($"AppId not confiugred for external service code {formTask.ExternalServiceCode}. Moving form task to error file.");
                MoveFormTaskToErrorFile(failedShipmentsFile, failedShipments, reportee, formTask);
                return true;
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

                string response = _client.PostInstance(appId.Split("/")[0], appId.Split("/")[1], multipartFormData).Result;

                SaveSentItem(sentItemsFileName, sentItems, formTask.SendersReference);

                _logger.LogInformation("Instance successfully created");
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

            return true;
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

        private SentItems GetSentItems(string sentItemsFileName)
        {
            if (!File.Exists(Path.Combine(_config.OutputFolder, sentItemsFileName)))
            {
                FileStream stream = File.Create(Path.Combine(_config.OutputFolder, sentItemsFileName));
                stream.Close();
                return new SentItems
                {
                    Reference = new()
                };
            }

            string json = File.ReadAllText(Path.Combine(_config.OutputFolder, sentItemsFileName));
            if (string.IsNullOrEmpty(json))
            {
                return new SentItems
                {
                    Reference = new()
                };
            }

            return JsonConvert.DeserializeObject<SentItems>(json);
        }

        private void SaveSentItem(string sentItemsFileName, SentItems sentItems, string sendersReference)
        {
            sentItems.Reference.Add(sendersReference);
            File.WriteAllText(Path.Combine(_config.OutputFolder, sentItemsFileName), JsonConvert.SerializeObject(sentItems));
        }
    }
}
