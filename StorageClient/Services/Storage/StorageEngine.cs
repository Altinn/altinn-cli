using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltinnCLI.Services.Storage
{
    public class StorageEngine : ApplicationEngineBase, IService, IHelp
    {
        public StorageEngine()
        {
        }


        public virtual void Run(string[] args)
        {

            BuildDependency(args);
            ParseArguments(args);
            ProcessCommand(args);
            Console.WriteLine("It's a me! Storage");
        }

        public Stream GetDocument(int instanceOwnerId, Guid instanceGuid, Guid dataId)
        {
            string baseAddress = System.Configuration.ConfigurationManager.AppSettings.Get("BaseAddress");

            IStorageClientWrapper wrapper = ServiceProvider.GetServices<IStorageClientWrapper>().FirstOrDefault();

            Stream documentStream = null;
            if (wrapper != null)
            {
                wrapper.BaseAddress = System.Configuration.ConfigurationManager.AppSettings.Get("BaseAddress");
                documentStream = wrapper.GetDocument(instanceOwnerId, instanceGuid, dataId);
            }
            else
            {

            }

            return documentStream;
        }

        public string Name
        {
            get
            {
                return "Storage";
            }
        }

        public string GetHelp()
        {
            return "Storage\nusage: storage <operation> -<option>\n\noperations:\ngetAttachment";
        }

        protected override IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            bool useLiveClient = ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>();

            if (useLiveClient)
            {
                services.AddTransient<IStorageClientWrapper, StorageClientWrapper>();
            }
            else
            {
                services.AddTransient<IStorageClientWrapper, StorageClientFileWrapper>();
            }
            return services;
        }

        private void ProcessCommand(string[] args)
        {
            ICommandHandler service = ApplicationManager.ServiceProvider.GetServices<ICommandHandler>().Where(s => string.Equals(s.Name, args[1], StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (service != null)
            {
                service.Run(args);
            }
            else
            {
                ApplicationManager.ServiceProvider.GetServices<IHelp>().FirstOrDefault().GetHelp();
            }
        }

        private KeyValuePair<string, string> ParseArguments(string[] args)
        {
            KeyValuePair<string, string> commandKeysAndValues = new KeyValuePair<string, string>();

            foreach(string parm in args)
            {
            }

            return commandKeysAndValues;
        }
    }
}
