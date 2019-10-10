using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StorageClient
{
    class Program
    {
        private static string prompt = "Altinn CLI > ";

        static void Main()
        {
            IServiceCollection services = GetServices();

            // Generate a provider
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IConfigurationRoot configuration = BuildConfiguration();

            ApplicationManager app = new ApplicationManager(serviceProvider, configuration);
            string args;            

            while(true)
            {
                Console.Write(prompt);
                args = Console.ReadLine();
                app.Execute(args);
            }
        }


        private static IServiceCollection GetServices()
        {
            IServiceCollection services = new ServiceCollection();

            // register all Services that can be accessed from commandline, they all implements the IService interface
            // that contains a name property that is used to select the properiate class according to cli command type, args[0]
            Assembly.GetEntryAssembly().GetTypesAssignableFrom<IService>().ForEach((t) =>
            {
                services.AddTransient(typeof(IService), t);
            });

            // register all Services that can be accessed from commandline, tey all implements the IHelp interface
            // that contains a name property that is used to select the properiate class according to cli command type, args[0]
            Assembly.GetEntryAssembly().GetTypesAssignableFrom<IHelp>().ForEach((t) =>
            {
                services.AddTransient(typeof(IHelp), t);
            });


            return services;
        }


        public static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            return builder.Build();

        }
    }
}
