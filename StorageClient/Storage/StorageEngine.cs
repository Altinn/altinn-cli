
using AltinnCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public class StorageEngine : ApplicationEngineBase, IApplicationEngine, IHelp
    {
        public StorageEngine()
        {
        }


        protected override IServiceCollection ConfigureServices(string applicationType)
        {
            IServiceCollection services = new ServiceCollection();

            bool useLiveClinet = ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>();

            if (useLiveClinet)
            {
                services.AddTransient<IStorageClientWrapper, StorageClientWrapper>();
            }
            else
            {
                services.AddTransient<IStorageClientWrapper, StorageClientFileWrapper>();
            }
            // IMPORTANT! Register our application entry point
            services.AddTransient<ApplicationEngineBase>();
            return services;
        }

        public override string Name
        {
            get
            {
                return "Storage";
            }
        }

        public string GetHelp()
        {
            return "Storage help";
        }

        public override void Execute(string[] args)
        {
            Console.WriteLine("It's a me! Storage");
        }
    }
}
