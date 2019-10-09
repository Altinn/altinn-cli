using AltinnCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageClient
{
    public abstract class ApplicationEngineBase
    {
        public IConfigurationRoot ApplicationConfiguration;
        public ServiceProvider ServiceProvider;
        public string[] Args;

        public abstract string Name { get; }

        public ApplicationEngineBase()
        {
        }

        public void Run(string[] args)
        {
            BuildConfiguration();
            // BuildDependency(args);

            // Exceute(args);
        }

        public virtual void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            ApplicationConfiguration = builder.Build();

        }

        public void BuildDependency(string[] args)
        {
            // Create service collection and configure our services
            IServiceCollection services = ConfigureServices(args[0]);
            // Generate a provider
            ServiceProvider = services.BuildServiceProvider();
        }

        protected abstract IServiceCollection ConfigureServices(string applicationType);

        public abstract void Execute(string[] args);
    }
}
