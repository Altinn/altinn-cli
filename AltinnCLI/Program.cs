using AltinnCLI.Core;
using AltinnCLI.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AltinnCLI
{
    /// <summary>
    ///  The CLI startup that prepares the application configuration, serviceprovider for handling cli-commands 
    /// </summary
    class Program
    {
        private static string prompt = "Altinn CLI > ";

        static void Main()
        {

            ConfigureLogging();
            IServiceCollection services = GetServices();

            // Generate a Name
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

        /// <summary>
        /// Finds all ICommand, ISubCommandHandler and  IHelp implemented klasses in running assembly and according to type
        /// registers them to be avilable through the Applications CommandProvider. 
        /// </summary>
        /// <returns>List of registred services</returns>
        private static IServiceCollection GetServices()
        {
            IServiceCollection services = new ServiceCollection();

            // register all Commands that can be accessed from commandline, they all implements the ICommand interface
            // that contains a name property that is used to select the properiate class according to cli command type, args[0]
            Assembly.GetEntryAssembly().GetTypesAssignableFrom<ICommand>().ForEach((t) =>
            {
                services.AddTransient(typeof(ICommand), t);
            });

            // register all Commands that can be accessed from commandline, they all implements the IHelp interface
            Assembly.GetEntryAssembly().GetTypesAssignableFrom<IHelp>().ForEach((t) =>
            {
                services.AddTransient(typeof(IHelp), t);
            });

            // register all services that implements the ISubCommandHandler interface, and in addtion add reference to the application logger
            Assembly.GetEntryAssembly().GetTypesAssignableFrom<ISubCommandHandler>().ForEach((t) =>
            {
                services.AddLogging(configure =>
                {
                    configure.ClearProviders();
                    configure.AddProvider(new SerilogLoggerProvider(Log.Logger));
                }).AddTransient(typeof(ISubCommandHandler),t);
            });

            return services;
        }

        /// <summary>
        /// Configure the logger
        /// </summary>
        private static void ConfigureLogging()
        {

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File("log.txt", LogEventLevel.Information)
               .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
               .CreateLogger();

        }

        /// <summary>
        /// Configure the application by loading the application settings file
        /// </summary>
        /// <returns>Top of the configuration hierarchy</returns>
        public static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            return builder.Build();

        }
    }
}
