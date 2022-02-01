using System;
using System.Net.Http;
using System.IO;
using System.Reflection;

using AltinnCLI.Commands.Core;
using AltinnCLI.Extensions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Altinn.ApiClients.Maskinporten.Extensions;
using Altinn.ApiClients.Maskinporten.Services;
using AltinnCLI.Configurations;
using AltinnCLI.Services;
using AltinnCLI.Services.Interfaces;

namespace AltinnCLI
{
    /// <summary>
    /// The CLI startup that prepares the application configuration, service provider for handling cli-commands 
    /// </summary
    class Program
    {
        private const string Prompt = "Altinn CLI > ";
        static void Main()
        {
            ConfigureLogging();

            IConfigurationRoot configuration = BuildConfiguration();

            IServiceCollection services = GetAndRegisterServices(configuration);

            ServiceProvider serviceProvider = services.BuildServiceProvider();


            var app = serviceProvider.GetService<ApplicationManager>();
            app.SetEnvironment(configuration, serviceProvider);

            while (true)
            {
                Console.Write(Prompt);
                string args = Console.ReadLine();
                try
                {
                    app.Execute(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($@"Error : {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Finds all classes implementing <see cref="ICommand"/>, <see cref="ISubCommandHandler"/> and <see cref="IHelp"/>
        /// in running assembly, and registers them as services according to type. This makes them available for the Applications CommandProvider. 
        /// </summary>
        /// <returns>List of registered services</returns>
        private static IServiceCollection GetAndRegisterServices(IConfigurationRoot configuration)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddProvider(new SerilogLoggerProvider(Log.Logger));
            }).AddTransient<ApplicationManager>();

            Assembly.GetEntryAssembly().GetTypesAssignableFrom<ICommand>().ForEach(command =>
            {
                services.AddLogging(configure =>
                {
                    configure.ClearProviders();
                    configure.AddProvider(new SerilogLoggerProvider(Log.Logger));
                }).AddTransient(typeof(ICommand), command);
            });

            Assembly.GetEntryAssembly().GetTypesAssignableFrom<IHelp>().ForEach(help =>
            {
                services.AddTransient(typeof(IHelp), help);
            });

            Assembly.GetEntryAssembly().GetTypesAssignableFrom<ISubCommandHandler>().ForEach(subCommand =>
            {
                services.AddLogging(configure =>
                {
                    configure.ClearProviders();
                    configure.AddProvider(new SerilogLoggerProvider(Log.Logger));
                }).AddTransient(typeof(ISubCommandHandler), subCommand);
            });

            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddHttpClient();

            services.AddMaskinportenHttpClient<SettingsJwkClientDefinition>(
                configuration.GetSection("MaskinportenSettings"),
                "myhttpclient");
            services.AddSingleton<IInstantiation,InstantiationService>();

             services.Configure<InstantiationConfig>(configuration.GetSection("InstantiationConfig"));

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
