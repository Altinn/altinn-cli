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

        public ServiceProvider ServiceProvider;
        public string[] Args;



        public ApplicationEngineBase()
        {
        }


        public void BuildDependency(string[] args)
        {
            // Create service collection and configure our services
            IServiceCollection services = ConfigureServices();
            // Generate a Name
            ServiceProvider = services.BuildServiceProvider();
        }

        protected abstract IServiceCollection ConfigureServices();

    }
}
