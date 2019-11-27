using AltinnCLI;
using AltinnCLI.Commands.Storage;
using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AltinnCLITest
{
    [TestClass]
    public class StorageTest
    {
        [TestMethod]
        public void Storage_GetData_Wrong_Option_Combination()
        {
            string expectedOption = "TestOption";
            string expectedDataType = "string";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";
            string expectedValue = "TestValue";

            string expectedLogMessage = "No valid combination";
            int expectedLogEntires = 1;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // fetch GetDataHandler subCommand
            ISubCommandHandler subCommandHandler = serviceProvider.GetService<GetDataHandler>();
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // assign option values to the subcommand
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

        }

        [TestMethod]
        public void Storage_GetData_No_Data()
        {
            string expectedOrgOption = "org";
            string expectedOrgDataType = "string";
            string expectedOrgDescription = "org";
            string expectedOrgApiName = "org";
            string expectedOrgValue = "5533";

            string expectedLogMessage = "No data available";
            int expectedLogEntires = 1;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOrgOption, expectedOrgDataType, expectedOrgDescription, expectedOrgApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOrgOption, expectedOrgValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // fetch GetDataHandler subCommand
            ISubCommandHandler subCommandHandler = serviceProvider.GetService<GetDataHandler>();
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // assign option values to the subcommand
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

        }


        [TestMethod]
        public void Storage_GetData_No_Data_For_Instance()
        {
            string expectedOrgOption = "org";
            string expectedOrgDataType = "string";
            string expectedOrgDescription = "org";
            string expectedOrgApiName = "org";
            string expectedOrgValue = "5533";

            string expectedFetchMessage = "Fetched 1 instances.";
            string expectedFileMessage = "No files received for instance:";
            int expectedLogEntires = 2;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOrgOption, expectedOrgDataType, expectedOrgDescription, expectedOrgApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOrgOption, expectedOrgValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // set respons from ClientWrapper
            InstanceResponseMessage respons = TestDataBuilder.CreateInstanceResponse(1);
            StorageClientFileWrapper.InstanceResponse = respons;

            // fetch GetDataHandler subCommand
            ISubCommandHandler subCommandHandler = serviceProvider.GetService<GetDataHandler>();
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // assign option values to the subcommand
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);

            string fetchMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFetchMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fetchMessage));

            string fileMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFileMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fileMessage));

        }


        [TestMethod]
        public void Storage_GetData_Data_For_Instance()
        {
            string expectedOrgOption = "org";
            string expectedOrgDataType = "string";
            string expectedOrgDescription = "org";
            string expectedOrgApiName = "org";
            string expectedOrgValue = "5533";

            string expectedFetchMessage = "Fetched 1 instances.";
            string expectedSaveMessage = "File:Kvittering";
            int expectedLogEntires = 2;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOrgOption, expectedOrgDataType, expectedOrgDescription, expectedOrgApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOrgOption, expectedOrgValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // set respons from ClientWrapper
            InstanceResponseMessage respons = TestDataBuilder.CreateInstanceResponse(1);
            StorageClientFileWrapper.InstanceResponse = respons;
            StorageClientFileWrapper.DataContent = new MemoryStream();

            // fetch GetDataHandler subCommand
            ISubCommandHandler subCommandHandler = serviceProvider.GetService<GetDataHandler>();
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // Mock the file wrapper
            Mock<IFileWrapper> mockedWrapper = new Mock<IFileWrapper>();
            mockedWrapper.Setup(x => x.SaveToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>())).Returns(true);
            subCommandHandler.CliFileWrapper = (IFileWrapper)mockedWrapper.Object;

            // assign option values to the subcommand
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);

            string fetchMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFetchMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fetchMessage));

            string saveMessage = logEntries.FirstOrDefault(x => x.Contains(expectedSaveMessage));
            Assert.IsFalse(string.IsNullOrEmpty(saveMessage));

        }

        private static void BuildEnvironment(string envirnonmentSetting)
        {
            byte[] data = Encoding.ASCII.GetBytes(envirnonmentSetting);
            MemoryStream stream = new MemoryStream(data);

            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(stream);
            IConfigurationRoot configurationRoot = configBuilder.Build();

            NullLogger<ApplicationManager> logger = new NullLogger<ApplicationManager>();
            ApplicationManager applicationManager = new ApplicationManager(logger);
            applicationManager.SetEnvironment(configurationRoot, null);

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

