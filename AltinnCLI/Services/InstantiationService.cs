using Altinn.Platform.Storage.Interface.Models;

using AltinnCLI.Configurations;
using AltinnCLI.Helpers;
using AltinnCLI.Models;
using AltinnCLI.Services.Interfaces;

using System;
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

        public InstantiationService(InstantiationConfig config)
        {
            _config = config;
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

                XmlSerializer serializer = new XmlSerializer(typeof(ServiceOwner));

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
                    InstanceOwner = new(),
                    DataValues = new()
                    {
                        { "SendersReference", formTask.SendersReference },
                        { "ReceiversReference", formTask.ReceiversReference },
                        { "IdentifyingFields", string.Join(", ", formTask.IdentifyingFields.ToList()) }
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

                // call a client and include the multipart form data.
                // string response = _clientWrapper.CreateInstance(appId, multipartFormData);
            }
            return Task.CompletedTask;
        }

        private MultipartFormDataContent BuildContentForInstance(Instance instance, ServiceOwnerPrefillReporteeFormTask formTask)
        {
            MultipartContentBuilder contentBuilder = new MultipartContentBuilder(instance);

            foreach (ServiceOwnerPrefillReporteeFormTaskForm form in formTask.Form)
            {
                string formData = form.FormData;
                StringContent stringContent = new StringContent(formData);
                stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
                contentBuilder.AddDataElement("default", stringContent);
            }

            return contentBuilder.Build();
        }
    }
}
