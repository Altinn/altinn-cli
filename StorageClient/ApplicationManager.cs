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

        private string _args { get; set; }

        public ApplicationManager(string args)
        {
            BuildDependency(args);

        private string[] processArgs()
        {
            return _args.Split(" ");
        }

        public void BuildDependency(string[] args)
        {
            // Create service collection and configure our services
            var services = ConfigureServices(args[0]);
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            // Kick off our actual code
            serviceProvider.GetService<IApplicationEngine>().Run(args);
        }

        private static IServiceCollection ConfigureServices(string applicationType)
        {
            IServiceCollection services = new ServiceCollection();

            switch (applicationType)
            {
                case "Storage":
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
