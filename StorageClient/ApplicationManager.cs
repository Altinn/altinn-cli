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

        private string[] processArgs(string args)
        {
            return args.ToLower().Split(" ");
        }

        public void Execute(string[] args)
        {
            var appServices = ServiceProvider.GetServices<IApplicationEngine>();
        }

        public void BuildDependency()
        {
            var builder = new HostBuilder().ConfigureServices()
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
                case "storage":
                    services.AddTransient<IService, StorageEngine>();
                    break;
                case "quit":
                    services.AddTransient<IService, QuitService>();
                    break;
                default:
                    services.AddTransient<IService, HelpService>();
                    break;
            }

            // IMPORTANT! Register our application entry point
            services.AddTransient<ApplicationManager>();
            return services;
        }
    }
}
