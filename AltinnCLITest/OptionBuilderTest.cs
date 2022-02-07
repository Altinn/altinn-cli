using AltinnCLI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AltinnCLI.Commands.Storage.SubCommandHandlers;
using AltinnCLI.Models;
using AltinnCLI.Commands.Core;
using AltinnCLI.Clients;
using System.Net.Http;

namespace AltinnCLITest
{
    [TestClass]
    public class OptionBuilderTest
    {
        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_No_Commands()
        {
            int expectedNumberOfOptions = 0;
            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            // Not commands in the Log
            CfgCommandList commandList = new();
            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new(
                 new DataClient(new HttpClient()),
                new InstanceClient(new HttpClient()), 
                new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_Command_Not_Found()
        {
            string expectedCommand = "NotDefinedCommand";
            int expectedNumberOfOptions = 0;
            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);

            CfgCommandList commandList = new();
            commandList.Commands = new List<CfgCommand>
            {
                command
            };

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new(
               new DataClient(new HttpClient()),
              new InstanceClient(new HttpClient()),
              new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_No_SubCommands()
        {
            string expectedCommand = "Storage";
            int expectedNumberOfOptions = 0;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);

            CfgCommandList commandList = new();
            commandList.Commands = new List<CfgCommand>
            {
                command
            };

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new(
                 new DataClient(new HttpClient()),
                new InstanceClient(new HttpClient()),
                new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_Wrong_SubCommand()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "TestCommand";

            int expectedNumberOfOptions = 0;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);

            CfgCommandList commandList = new();
            commandList.Commands = new List<CfgCommand>
            {
                command
            };

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new(
               new DataClient(new HttpClient()),
              new InstanceClient(new HttpClient()),
              new NullLogger<GetDataHandler>());
            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_No_Options()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "GetData";

            int expectedNumberOfOptions = 0;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);

            CfgCommandList commandList = new();
            commandList.Commands = new List<CfgCommand>
            {
                command
            };

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

             GetDataHandler handler = new(
                 new DataClient(new HttpClient()),
                new InstanceClient(new HttpClient()), 
                new NullLogger<GetDataHandler>());
            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_One_Option()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "GetData";
            string expectedOption = "TestOption";
            string expectedDataType = "string";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";

            int expectedNumberOfOptions = 1;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);
            CfgOption option = TestDataBuilder.CreateCfgOption(expectedOption, expectedDataType, expectedDescription, expectedApiName);
            subCommand.Options.Add(option);

            CfgCommandList commandList = new();
            commandList.Commands = new List<CfgCommand>
            {
                command
            };

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new(
               new DataClient(new HttpClient()),
              new InstanceClient(new HttpClient()),
              new NullLogger<GetDataHandler>());
            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
            IOption selectableOption = selectableOptions[0];

            Assert.AreEqual(expectedOption, selectableOption.Name);
            Assert.AreEqual(expectedDescription, selectableOption.Description);
            Assert.AreEqual(expectedApiName, selectableOption.ApiName);
            Assert.IsFalse(selectableOption.IsAssigned);
            Assert.IsInstanceOfType(selectableOption, typeof(NumberOption<string>));
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_Wrong_Option_type()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "GetData";
            string expectedOption = "TestOption";
            string expectedDataType = "testType";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";

            int expectedNumberOfOptions = 0;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new();

            BuildEnvironment(environmentSetting);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);
            CfgOption option = TestDataBuilder.CreateCfgOption(expectedOption, expectedDataType, expectedDescription, expectedApiName);
            subCommand.Options.Add(option);

            CfgCommandList commandList = new();
            commandList.Commands = new List<CfgCommand>
            {
                command
            };

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new(
                 new DataClient(new HttpClient()),
                new InstanceClient(new HttpClient()),
                new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_One_Option_AssignValue()
        {
            string expectedOption = "TestOption";
            string expectedDataType = "string";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";
            string expectedValue = "TestValue";

            int expectedNumberOfOptions = 1;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<GetDataHandler> logger = new();

            BuildEnvironment(environmentSetting);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new();
            cliOptions.Add(expectedOption, expectedValue);

            ISubCommandHandler subCommandHandler = new GetDataHandler(
                 new DataClient(new HttpClient()),
                new InstanceClient(new HttpClient()),
                logger)
            {
                SelectableCliOptions = selectableOptions,
                DictOptions = cliOptions
            };

            bool isValid = builder.AssignValueToCliOptions(subCommandHandler);

            Assert.IsTrue(isValid);
            Assert.AreEqual(expectedNumberOfOptions, subCommandHandler.SelectableCliOptions.Count);
            IOption selectableOption = subCommandHandler.SelectableCliOptions[0];

            Assert.AreEqual(expectedOption, selectableOption.Name);
            Assert.AreEqual(expectedDescription, selectableOption.Description);
            Assert.AreEqual(expectedApiName, selectableOption.ApiName);
            Assert.IsTrue(selectableOption.IsAssigned);
            Assert.AreEqual(expectedValue, selectableOption.Value);

            Assert.IsInstanceOfType(selectableOption, typeof(NumberOption<String>));
        }

        [TestMethod]
        [DoNotParallelize]
        public void OptionBuilder_CreateOption_Command_Found_One_Option_AssignValue_Wrong_Input_Value()
        {
            string expectedOption = "TestOption";
            string expectedDataType = "Guid";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";
            string expectedValue = "TestValue";

            int expectedNumberOfOptions = 1;

            string environmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<GetDataHandler> logger = new();

            BuildEnvironment(environmentSetting);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new();
            cliOptions.Add(expectedOption, expectedValue);

            ISubCommandHandler subCommandHandler = new GetDataHandler(
                 new DataClient(new HttpClient()),
                new InstanceClient(new HttpClient()),
                logger)
            {
                SelectableCliOptions = selectableOptions,
                DictOptions = cliOptions
            };



            bool isValid = builder.AssignValueToCliOptions(subCommandHandler);

            Assert.IsFalse(isValid);
            Assert.AreEqual(expectedNumberOfOptions, subCommandHandler.SelectableCliOptions.Count);
            IOption selectableOption = subCommandHandler.SelectableCliOptions[0];

            Assert.AreEqual(expectedOption, selectableOption.Name);
            Assert.AreEqual(expectedDescription, selectableOption.Description);
            Assert.AreEqual(expectedApiName, selectableOption.ApiName);
            Assert.IsTrue(selectableOption.IsAssigned);
            Assert.IsFalse(string.IsNullOrEmpty(selectableOption.ErrorMessage));
        }

        private static void BuildEnvironment(string environmentSetting)
        {
            byte[] data = Encoding.ASCII.GetBytes(environmentSetting);
            MemoryStream stream = new(data);

            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(stream);
            IConfigurationRoot configurationRoot = configBuilder.Build();

            ConfigureLogging();

            NullLogger<ApplicationManager> logger = new();
            ApplicationManager applicationManager = new(logger);
            applicationManager.SetEnvironment(configurationRoot, null);
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();
        }
    }
}
