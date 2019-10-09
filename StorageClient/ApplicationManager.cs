using AltinnCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageClient
{
    public class ApplicationManager
    {
        public ApplicationManager(string args)
        {
            BuildDependency(processArgs(args));
        }

        private string[] processArgs(string args)
        {
            return args.ToLower().Split(" ");
        }

        public void BuildDependency(string[] args)
        {
            var builder = new HostBuilder().ConfigureServices()
            // Create service collection and configure our services
            var services = ConfigureServices(args[0]);
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            // Kick off our actual code
            serviceProvider.GetService<IService>().Run(args);
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
