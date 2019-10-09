using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageClient
{
    public abstract class ApplicationEngineBase
    {
        public IConfigurationRoot ApplicationConfiguration;

        public ApplicationEngineBase()
        {
        }

        public virtual void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            IConfigurationRoot ApplicationConfiguration = builder.Build();

        }

        public virtual void BuildDependency()
        {

        }
    }
}
