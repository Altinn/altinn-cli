using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AltinnCLI;
using AltinnCLI.Commands.Storage;
using AltinnCLI.Commands.Storage.SubCommandHandlers;
using AltinnCLI.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Serilog;
using Serilog.Events;

namespace AltinnCLITest
{
    [TestClass]
    public class StorageTest
    {
        [TestCleanup]
        public void CleanUp()
        {
            StorageClientFileWrapper.InstanceResponse = null;
            StorageClientFileWrapper.IsSuccessStatusCode = false;
            StorageClientFileWrapper.DataContent = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_GetData_Wrong_Option_Combination()
        {
            string expectedOption = "TestOption";
            string expectedDataType = "string";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";
            string expectedValue = "TestValue";

            string expectedLogMessage = "No valid combination";
            int expectedLogEntries = 1;

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Fetch GetDataHandler subCommand
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "GetData");
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // Assign option values to the sub command
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_GetData_No_Data()
        {
            string expectedOrgOption = "org";
            string expectedOrgDataType = "string";
            string expectedOrgDescription = "org";
            string expectedOrgApiName = "org";
            string expectedOrgValue = "5533";

            string expectedLogMessage = "No data available";
            int expectedLogEntries = 1;

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOrgOption, expectedOrgDataType, expectedOrgDescription, expectedOrgApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOrgOption, expectedOrgValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Fetch GetDataHandler subCommand
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "GetData");
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // Assign option values to the sub command
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_GetData_No_Data_For_Instance()
        {
            string expectedOrgOption = "org";
            string expectedOrgDataType = "string";
            string expectedOrgDescription = "org";
            string expectedOrgApiName = "org";
            string expectedOrgValue = "5533";

            string expectedFetchMessage = "Fetched 1 instances.";
            string expectedFileMessage = "No files received for instance:";
            int expectedLogEntries = 2;

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOrgOption, expectedOrgDataType, expectedOrgDescription, expectedOrgApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOrgOption, expectedOrgValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Set response from ClientWrapper
            InstanceResponseMessage response = TestDataBuilder.CreateInstanceResponse(1);
            StorageClientFileWrapper.InstanceResponse = response;

            // Fetch GetDataHandler sub command
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "GetData");
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // Assign option values to the sub command
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);

            string fetchMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFetchMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fetchMessage));

            string fileMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFileMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fileMessage));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_GetData_Data_For_Instance()
        {
            string expectedOrgOption = "org";
            string expectedOrgDataType = "string";
            string expectedOrgDescription = "org";
            string expectedOrgApiName = "org";
            string expectedOrgValue = "5533";

            string expectedFetchMessage = "Fetched 1 instances.";
            string expectedSaveMessage = "File:Kvittering";
            int expectedLogEntries = 2;

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOrgOption, expectedOrgDataType, expectedOrgDescription, expectedOrgApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOrgOption, expectedOrgValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Set response from ClientWrapper
            InstanceResponseMessage response = TestDataBuilder.CreateInstanceResponse(1);
            StorageClientFileWrapper.InstanceResponse = response;
            StorageClientFileWrapper.DataContent = new MemoryStream();

            // Fetch GetDataHandler subCommand
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "GetData");
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            // Mock the file wrapper
            Mock<IFileWrapper> mockedWrapper = new Mock<IFileWrapper>();
            mockedWrapper.Setup(x => x.SaveToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>())).Returns(true);
            subCommandHandler.CliFileWrapper = mockedWrapper.Object;

            // Assign option values to the sub command
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);

            string fetchMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFetchMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fetchMessage));

            string saveMessage = logEntries.FirstOrDefault(x => x.Contains(expectedSaveMessage));
            Assert.IsFalse(string.IsNullOrEmpty(saveMessage));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_UploadData_Wrong_Option_Combination()
        {
            string expectedOption = "ownerId";
            string expectedValue = "50013748";

            string expectedLogMessage = "No valid combination";
            int expectedLogEntries = 1;

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Fetch GetDataHandler subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Assign option values to the sub command
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_UploadData_No_File()
        {
            string expectedOwnerId = "ownerId";
            string expectedOwnerIdValue = "50013748";

            string expectedInstanceId = "instanceId";
            string expectedInstanceIdValue = "bd5d5066-5ae4-42eb-8d5d-076a600acdd5";

            string expectedElementType = "elementType";
            string expectedElementTypeValue = "kvittering";

            string expectedFile = "file";
            string expectedFileValue = @"c:\Temp\test.xml";

            string expectedLogMessage = "No valid combination";
            int expectedLogEntries = 2;

            string expectedFileNotFoundErrorMessage = "Invalid value for parameter";

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOwnerId, expectedOwnerIdValue);
            cliOptions.Add(expectedInstanceId, expectedInstanceIdValue);
            cliOptions.Add(expectedElementType, expectedElementTypeValue);
            cliOptions.Add(expectedFile, expectedFileValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Fetch UploadData subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Need to mock the FileOption to avoid dependency to disk, so replace the registered option with a mock option 
            FileOption<FileStream> mockFileOption = Mock.Of<FileOption<FileStream>>(x => x.Validate() == false && x.ErrorMessage == expectedFileNotFoundErrorMessage);
            ReplaceSelectableOption("file", mockFileOption, subCommandHandler.SelectableCliOptions);

            // Assign the input options to the subCommand
            subCommandHandler.DictOptions = cliOptions;

            // Assign and validate the input options to the selectable options
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));
            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

            string fileNotFoundMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFileNotFoundErrorMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fileNotFoundMessage));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_UploadData__File_Exits_FailedTo_Upload()
        {
            string expectedOwnerId = "ownerId";
            string expectedOwnerIdValue = "50013748";

            string expectedInstanceId = "instanceId";
            string expectedInstanceIdValue = "bd5d5066-5ae4-42eb-8d5d-076a600acdd5";

            string expectedElementType = "elementType";
            string expectedElementTypeValue = "kvittering";

            string expectedFile = "file";
            string expectedFileValue = @"c:\Temp\test.xml";

            int expectedLogEntries = 1;

            string expectedUploadFailedMessage = "Failed to upload";

            //Build environment, 
            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registered classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOwnerId, expectedOwnerIdValue);
            cliOptions.Add(expectedInstanceId, expectedInstanceIdValue);
            cliOptions.Add(expectedElementType, expectedElementTypeValue);
            cliOptions.Add(expectedFile, expectedFileValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Fetch UploadData subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Need to mock the FileOption to avoid dependency to disk, so replace the registered option with a mock option 
            FileOption<FileStream> mockFileOption = Mock.Of<FileOption<FileStream>>(x => x.Validate());
            ReplaceSelectableOption("file", mockFileOption, subCommandHandler.SelectableCliOptions);

            // Mock the file wrapper
            Mock<IFileWrapper> mockedWrapper = new Mock<IFileWrapper>();
            mockedWrapper.Setup(x => x.GetFile(It.IsAny<string>())).Returns(new MemoryStream());
            subCommandHandler.CliFileWrapper = mockedWrapper.Object;

            // Set response from ClientWrapper
            StorageClientFileWrapper.IsSuccessStatusCode = false;

            // Assign the input options to the subCommand
            subCommandHandler.DictOptions = cliOptions;

            // Assign and validate the input options to the selectable options
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedUploadFailedMessage));
            Assert.IsFalse(string.IsNullOrEmpty(logMessage));
            
            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        [TestMethod]
        [DoNotParallelize]
        public void Storage_UploadData__File_Exits_File_Uploaded()
        {
            string expectedOwnerId = "ownerId";
            string expectedOwnerIdValue = "50013748";

            string expectedInstanceId = "instanceId";
            string expectedInstanceIdValue = "bd5d5066-5ae4-42eb-8d5d-076a600acdd5";

            string expectedElementType = "elementType";
            string expectedElementTypeValue = "kvittering";

            string expectedFile = "file";
            string expectedFileValue = @"c:\Temp\test.xml";

            int expectedLogEntries = 1;

            string expectedFileUploadedMessage = "was successfully uploaded";

            string environmentSetting = "{\"UseLiveClient\": \"false\"}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(environmentSetting);

            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOwnerId, expectedOwnerIdValue);
            cliOptions.Add(expectedInstanceId, expectedInstanceIdValue);
            cliOptions.Add(expectedElementType, expectedElementTypeValue);
            cliOptions.Add(expectedFile, expectedFileValue);

            // Define which command and sub command that shall be registered in service provider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // Register commands and sub commands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // Fetch UploadData subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.First(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Need to mock the FileOption to avoid dependency to disk, so replace the registered option with a mock option 
            FileOption<FileStream> mockFileOption = Mock.Of<FileOption<FileStream>>(x => x.Validate());
            ReplaceSelectableOption("file", mockFileOption, subCommandHandler.SelectableCliOptions);

            // Mock the file wrapper
            Mock<IFileWrapper> mockedWrapper = new Mock<IFileWrapper>();
            mockedWrapper.Setup(x => x.GetFile(It.IsAny<string>())).Returns(new MemoryStream());
            subCommandHandler.CliFileWrapper = mockedWrapper.Object;

            // Set response from ClientWrapper
            StorageClientFileWrapper.IsSuccessStatusCode = true;

            // Assign the input options to the subCommand
            subCommandHandler.DictOptions = cliOptions;

            // Assign and validate the input options to the selectable options
            builder.AssignValueToCliOptions(subCommandHandler);

            // Run the command
            subCommandHandler.Run();

            // Verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntries, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedFileUploadedMessage));
            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

            Assert.IsTrue(logMessage.Contains(expectedFileValue));

            textWriter.Dispose();
            builder.CfgCommands = null;
        }

        private static void ReplaceSelectableOption(string replaceOptionName, FileOption<FileStream> newOption, List<IOption> selectableOptions)
        {
            IOption currentOption = selectableOptions.FirstOrDefault(x => x.Name == replaceOptionName);
            if (currentOption != null)
            {
                newOption.ApiName = currentOption.ApiName;
                newOption.IsAssigned = false;
                newOption.Name = currentOption.Name;
                selectableOptions.Remove(currentOption);
                selectableOptions.Add(newOption);
            }
        }

        private static void BuildEnvironment(string environmentSetting)
        {
            byte[] data = Encoding.ASCII.GetBytes(environmentSetting);
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
               .WriteTo.TextWriter(textWriter, LogEventLevel.Information)
               .CreateLogger();
        }

        private static List<string> GetLogEntries(TextWriter textWriter)
        {
            List<string> logEntries = new List<string>();
            StringReader re = new StringReader(textWriter.ToString());
            string input;
            while ((input = re.ReadLine()) != null)
            {
                logEntries.Add(input);
            }

            return logEntries;
        }
    }
}

