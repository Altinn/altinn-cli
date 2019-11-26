using AltinnCLI;
using AltinnCLI.Commands.Storage;
using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AltinnCLITest
{
    [TestClass]
    public class ApplicationManagerTest
    {
        [TestMethod]
        public void ApplicationManager_Execute_NoDefined_Commands()
        {
            int expectedLogEntires = 1;
            
            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
 
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            IConfigurationRoot appConfig = BuildEnvironment(envirnonmentSetting);

            List<Type> availableCommandTypes = new List<Type>();
            List<Type> availableSubCommands = new List<Type>();

            ServiceProvider serviceProvider = BuildServiceProvider(availableCommandTypes, availableSubCommands);

            ApplicationManager applicationManager = serviceProvider.GetService<ApplicationManager>();
            applicationManager.SetEnvironment(appConfig, serviceProvider);

            string args = $"storage appId=tdd/apptest processIsComplete=true";

            applicationManager.Execute(args);

            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string noCommandsFound = logEntries.FirstOrDefault(x => x.Contains("No commands found"));

            Assert.IsFalse(string.IsNullOrEmpty(noCommandsFound));

        }

        [TestMethod]
        public void ApplicationManager_Execute_Defined_Command_No_Sub_Command()
        {
            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";

            IConfigurationRoot appConfig = BuildEnvironment(envirnonmentSetting);

            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();

            ServiceProvider serviceProvider = BuildServiceProvider(availableCommandTypes, availableSubCommands);

            ApplicationManager applicationManager = serviceProvider.GetService<ApplicationManager>();
            applicationManager.SetEnvironment(appConfig, serviceProvider);

            string args = $"storage appId=tdd/apptest processIsComplete=true";

            applicationManager.Execute(args);

        }


        [TestMethod]
        public void ApplicationManager_Execute_Defined_Command()
        {
            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
 
            IConfigurationRoot appConfig = BuildEnvironment(envirnonmentSetting);

            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            ServiceProvider serviceProvider = BuildServiceProvider(availableCommandTypes, availableSubCommands);

            ApplicationManager applicationManager = serviceProvider.GetService<ApplicationManager>();
            applicationManager.SetEnvironment(appConfig, serviceProvider);

            string args = $"storage appId=tdd/apptest processIsComplete=true";

            applicationManager.Execute(args);

        }

        private static IConfigurationRoot BuildEnvironment(string envirnonmentSetting)
        {
            byte[] data = Encoding.ASCII.GetBytes(envirnonmentSetting);
            MemoryStream stream = new MemoryStream(data);

            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(stream);
            IConfigurationRoot configurationRoot = configBuilder.Build();

            return configurationRoot;
        }

        private static ServiceProvider BuildServiceProvider(List<Type> availableCommandTypes, List<Type> availableSubCommands)
        { 


            IServiceCollection services = new ServiceCollection();

            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddProvider(new SerilogLoggerProvider(Log.Logger));
            }).AddTransient<ApplicationManager>();


            availableCommandTypes.ForEach((t) =>
            {
                services.AddTransient(typeof(ICommand), t);
            });

            availableSubCommands.ForEach((t) =>
             {
                 services.AddLogging(configure =>
                 {
                     configure.ClearProviders();
                     configure.AddProvider(new SerilogLoggerProvider(Log.Logger));
                 }).AddTransient(typeof(ISubCommandHandler), t);
             });

            ServiceProvider provider = services.BuildServiceProvider();

            return provider;
        }

        private static void ConfigureLogging(TextWriter textWriter)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File("log.txt", LogEventLevel.Information)
               .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
               .WriteTo.TextWriter(textWriter, LogEventLevel.Information)
               .CreateLogger();
        }

        private static List<string> GetLogEntries(TextWriter textWriter)
        {
            List<string> logEntries = new List<string>();
            StringReader re = new StringReader(textWriter.ToString());
            string input = null;
            while ((input = re.ReadLine()) != null)
            {
                logEntries.Add(input);
            }

            return logEntries;
        }

    }
}