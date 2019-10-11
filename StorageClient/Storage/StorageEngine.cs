
using AltinnCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public class StorageEngine : ApplicationEngineBase, IService, IHelp
    {
        public StorageEngine()
        {
        }


        protected override IServiceCollection ConfigureServices(string applicationType)
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
            // IMPORTANT! Register our application entry point
            //Type t = this.GetType();
            //services.AddTransient(t, t);
            return services;
        }

        public string Provider
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

        public virtual void Run(string[] args)
        {

            BuildDependency(args);

            Console.WriteLine("It's a me! Storage");
        }
    }
}
