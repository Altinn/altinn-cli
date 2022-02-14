using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AltinnCLI;
using AltinnCLI.Commands.Storage;
using AltinnCLI.Commands.Storage.SubCommandHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog;
using Serilog.Events;

namespace AltinnCLITest
{
    [TestClass]
    public class ApplicationManagerTest
    {
        [TestMethod]
        public void ApplicationManager_Execute_NoDefined_Commands()
        {
            int expectedLogEntries = 1;
            string expectedLogMessage = "No commands found";

            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            IConfigurationRoot appConfig = BuildEnvironment();

            List<Type> availableCommandTypes = new();
            List<Type> availableSubCommands = new();

            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            ApplicationManager applicationManager = serviceProvider.GetService<ApplicationManager>();
            applicationManager.SetEnvironment(appConfig, serviceProvider);
            ApplicationManager.IsLoggedIn = true;
            string args = $"storage GetData appId=tdd/apptest processIsComplete=true";

            applicationManager.Execute(args);

            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string noCommandsFound = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(noCommandsFound));

            textWriter.Dispose();
        }

        [TestMethod]
        public void ApplicationManager_Execute_Defined_Command_No_Sub_Command()
        {
            int expectedLogEntries = 2;
            string expectedLogMessage = "Missing sub command";
            string expectedLogMissingHelp = "Help is not found";

            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            IConfigurationRoot appConfig = BuildEnvironment();

            List<Type> availableCommandTypes = new();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new();

            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            ApplicationManager applicationManager = serviceProvider.GetService<ApplicationManager>();
            applicationManager.SetEnvironment(appConfig, serviceProvider);
            ApplicationManager.IsLoggedIn = true;

            string args = $"storage GetData appId=tdd/apptest processIsComplete=true";

            applicationManager.Execute(args);

            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);

            string missingHelp = logEntries.FirstOrDefault(x => x.Contains(expectedLogMissingHelp));
            Assert.IsFalse(string.IsNullOrEmpty(missingHelp));

            string missingSubCommand = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));
            Assert.IsFalse(string.IsNullOrEmpty(missingSubCommand));

            textWriter.Dispose();
        }

        private static IConfigurationRoot BuildEnvironment()
        {
            var configBuilder = new ConfigurationBuilder();
            IConfigurationRoot configurationRoot = configBuilder.Build();

            return configurationRoot;
        }

        private static void ConfigureLogging(TextWriter textWriter)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TextWriter(textWriter, LogEventLevel.Information)
                .CreateLogger();
        }

        private static List<string> GetLogEntries(TextWriter textWriter)
        {
            List<string> logEntries = new();
            StringReader re = new(textWriter.ToString());
            string input;
            while ((input = re.ReadLine()) != null)
            {
                logEntries.Add(input);
            }

            return logEntries;
        }
    }
}