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

        private string[] processArgs(string args)
        {
            return args.ToLower().Split(" ");
        }

        public void Execute(string args)
        {
            string[] input = processArgs(args);
            ServiceProvider.GetService<IApplicationEngine>().Execute(input);
        }

        public void BuildDependency()
        {
            // Create service collection and configure our services
            IServiceCollection services = ConfigureServices();
            // Generate a provider
            ServiceProvider = services.BuildServiceProvider();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<IApplicationEngine, StorageEngine>();
            services.AddTransient<IService, QuitService>();     

            // IMPORTANT! Register our application entry point
            services.AddTransient<ApplicationManager>();
            return services;
        }
    }
}
