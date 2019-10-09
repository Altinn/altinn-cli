using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public class StorageEngine : ApplicationEngineBase
    {
        public StorageEngine()
        {
        }

        public override void BuildConfiguration()
        {
            string baseAddresspath = ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            bool useLiveClinet = ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>();
        }

        public override void BuildDependency()
        {
            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            // Kick off our actual code
            serviceProvider.GetService<ConsoleApplication>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<ITestService, TestService>();
            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }

}
