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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            // fetch GetDataHandler subCommand
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "GetData");
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
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "GetData");
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
            Type tt = typeof(GetDataHandler);
            availableSubCommands.Add(typeof(GetDataHandler));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // set respons from ClientWrapper
            InstanceResponseMessage respons = TestDataBuilder.CreateInstanceResponse(1);
            StorageClientFileWrapper.InstanceResponse = respons;

            // fetch GetDataHandler subCommand
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "GetData");
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
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "GetData");
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
            int expectedLogEntires = 1;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // fetch GetDataHandler subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
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
            string expectedFileValue = $@"c:\Temp\test.xml";

            string expectedLogMessage = "No valid combination";
            int expectedLogEntires = 2;

            string expectedfileNotFoundErrorMessage = "Invalid value for parameter";

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOwnerId, expectedOwnerIdValue);
            cliOptions.Add(expectedInstanceId, expectedInstanceIdValue);
            cliOptions.Add(expectedElementType, expectedElementTypeValue);
            cliOptions.Add(expectedFile, expectedFileValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // fetch UploadData subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Need to mock the FileOption to avoid dependency to disk, so replace the registred option with a mockoption 
            FileOption<FileStream> mockFileOption = Mock.Of<FileOption<FileStream>>(x => x.Validate() == false && x.ErrorMessage == expectedfileNotFoundErrorMessage);
            ReplaceSelectableOption("file", mockFileOption, subCommandHandler.SelectableCliOptions);

            // assing the input options to the subCommand
            subCommandHandler.DictOptions = cliOptions;

            // assign and validate the input options to the selectable options
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));
            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

            string fileNotFoundMesage = logEntries.FirstOrDefault(x => x.Contains(expectedfileNotFoundErrorMessage));
            Assert.IsFalse(string.IsNullOrEmpty(fileNotFoundMesage));

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
            string expectedFileValue = $@"c:\Temp\test.xml";

            int expectedLogEntires = 1;

            string expectededUploadFailedMessage = "Failed to upload";

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOwnerId, expectedOwnerIdValue);
            cliOptions.Add(expectedInstanceId, expectedInstanceIdValue);
            cliOptions.Add(expectedElementType, expectedElementTypeValue);
            cliOptions.Add(expectedFile, expectedFileValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // fetch UploadData subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Need to mock the FileOption to avoid dependency to disk, so replace the registred option with a mockoption 
            FileOption<FileStream> mockFileOption = Mock.Of<FileOption<FileStream>>(x => x.Validate() == true);
            ReplaceSelectableOption("file", mockFileOption, subCommandHandler.SelectableCliOptions);

            // Mock the file wrapper
            Mock<IFileWrapper> mockedWrapper = new Mock<IFileWrapper>();
            mockedWrapper.Setup(x => x.GetFile(It.IsAny<string>())).Returns(new MemoryStream());
            subCommandHandler.CliFileWrapper = (IFileWrapper)mockedWrapper.Object;

            // set respons from ClientWrapper
            StorageClientFileWrapper.IsSuccessStatusCode = false;

            // assing the input options to the subCommand
            subCommandHandler.DictOptions = cliOptions;

            // assign and validate the input options to the selectable options
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectededUploadFailedMessage));
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
            string expectedFileValue = $@"c:\Temp\test.xml";

            int expectedLogEntires = 1;

            string expectededFileUploadedMessage = "was successfully uploaded";

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"false\"}}";
            NullLogger<OptionBuilder> logger = new NullLogger<OptionBuilder>();

            // Configure logger which is set on registred classes/objects
            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);

            BuildEnvironment(envirnonmentSetting);

            // Build command options
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOwnerId, expectedOwnerIdValue);
            cliOptions.Add(expectedInstanceId, expectedInstanceIdValue);
            cliOptions.Add(expectedElementType, expectedElementTypeValue);
            cliOptions.Add(expectedFile, expectedFileValue);

            // define which command and subcommand that shall be registred in serviceprovider
            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(UploadData));

            // register commands and subcommands
            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);

            // fetch UploadData subCommand and assign available options by use og OptionBuilder. Options for the command is fetched from
            // the Command resource file defined in the Cli project
            var sssList = serviceProvider.GetServices<ISubCommandHandler>().ToList();
            ISubCommandHandler subCommandHandler = sssList.FirstOrDefault(x => x.Name == "UploadData");
            OptionBuilder builder = OptionBuilder.Instance(logger);
            subCommandHandler.SelectableCliOptions = builder.BuildAvailableOptions(subCommandHandler);
            subCommandHandler.DictOptions = cliOptions;

            // Need to mock the FileOption to avoid dependency to disk, so replace the registred option with a mockoption 
            FileOption<FileStream> mockFileOption = Mock.Of<FileOption<FileStream>>(x => x.Validate() == true);
            ReplaceSelectableOption("file", mockFileOption, subCommandHandler.SelectableCliOptions);

            // Mock the file wrapper
            Mock<IFileWrapper> mockedWrapper = new Mock<IFileWrapper>();
            mockedWrapper.Setup(x => x.GetFile(It.IsAny<string>())).Returns(new MemoryStream());
            subCommandHandler.CliFileWrapper = (IFileWrapper)mockedWrapper.Object;

            // set respons from ClientWrapper
            StorageClientFileWrapper.IsSuccessStatusCode = true;

            // assing the input options to the subCommand
            subCommandHandler.DictOptions = cliOptions;

            // assign and validate the input options to the selectable options
            builder.AssignValueToCliOptions(subCommandHandler);

            // run the command
            subCommandHandler.Run();

            // verify that the log contain expected result
            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectededFileUploadedMessage));
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

