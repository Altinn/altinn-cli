using AltinnCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageClient
{
    public class ApplicationManager
    {

        private ServiceProvider ServiceProvider;

        public ApplicationManager()
        {
            BuildDependency();

        }

        public void Execute(string[] args)
        {
            var appServices = ServiceProvider.GetServices<IApplicationEngine>();
        }

        public void BuildDependency()
        {
            // Create service collection and configure our services
            IServiceCollection services = ConfigureServices(args[0]);
        // Generate a provider
            ServiceProvider = services.BuildServiceProvider();

            // Kick off our actual code
            serviceProvider.GetService<IApplicationEngine>().Run(args);

            var appServices = serviceProvider.GetServices<IApplicationEngine>();

        /// SELECT FROM

        }

        private static IServiceCollection ConfigureServices(string applicationType)
        {
            IServiceCollection services = new ServiceCollection();

            switch (applicationType)
            {
                case "Storage":
                // Define link in registration
                    services.AddTransient<IApplicationEngine, StorageEngine>();
                    break;
                default:
                    break;
            }

            // IMPORTANT! Register our application entry point
            services.AddTransient<ApplicationManager>();
            return services;
        }
    }
}
