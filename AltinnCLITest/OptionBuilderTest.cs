using AltinnCLI;
using AltinnCLI.Commands.Storage;
using AltinnCLI.Core;
using AltinnCLI.Core.Json;
using JsonFx.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLITest
{
    [TestClass]
    public class OptionBuilderTest
    {
        [TestMethod]
        public void OptionBuilder_CreateOption_No_Commands()
        {
            int expectedNumberOfOptions = 0;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            // Not commands in the Log
            CfgCommandList commandList = new CfgCommandList();
            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;

            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }


        [TestMethod]
        public void OptionBuilder_Command_Not_Found()
        {
            string expectedCommand = "NotDefinedCommand";
            int expectedNumberOfOptions = 0;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);

            CfgCommandList commandList = new CfgCommandList();
            commandList.Commands = new List<CfgCommand>();
            commandList.Commands.Add(command);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;


            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_No_SubCommands()
        {
            string expectedCommand = "Storage";

            int expectedNumberOfOptions = 0;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);

            CfgCommandList commandList = new CfgCommandList();
            commandList.Commands = new List<CfgCommand>();
            commandList.Commands.Add(command);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;


            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_Wrong_SubCommand()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "TestCommand";

            int expectedNumberOfOptions = 0;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);

            CfgCommandList commandList = new CfgCommandList();
            commandList.Commands = new List<CfgCommand>();
            commandList.Commands.Add(command);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;


            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_No_Options()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "GetData";

            int expectedNumberOfOptions = 0;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);
        
            CfgCommandList commandList = new CfgCommandList();
            commandList.Commands = new List<CfgCommand>();
            commandList.Commands.Add(command);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;


            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }
               
        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_One_Option()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "GetData";
            string expectedOption = "TestOption";
            string expectedDataType = "string";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";

            int expectedNumberOfOptions = 1;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);
            CfgOption option = TestDataBuilder.CreateCfgOption(expectedOption, expectedDataType, expectedDescription, expectedApiName);
            subCommand.Options.Add(option);


            CfgCommandList commandList = new CfgCommandList();
            commandList.Commands = new List<CfgCommand>();
            commandList.Commands.Add(command);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;


            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
            IOption selectableoption = selectableOptions[0];

            Assert.AreEqual(expectedOption, selectableoption.Name);
            Assert.AreEqual(expectedDescription, selectableoption.Description);
            Assert.AreEqual(expectedApiName, selectableoption.ApiName);
            Assert.IsFalse(selectableoption.IsAssigned);
            Assert.IsInstanceOfType(selectableoption, typeof(NumberOption<String>));

        }

        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_Wrong_Option_type()
        {
            string expectedCommand = "Storage";
            string expectedSubCommand = "GetData";
            string expectedOption = "TestOption";
            string expectedDataType = "testType";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";

            int expectedNumberOfOptions = 0;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            CfgCommand command = TestDataBuilder.CreateCfgCommand(expectedCommand);
            CfgSubCommand subCommand = TestDataBuilder.CreateCfgSubCommand(expectedSubCommand);
            command.SubCommands.Add(subCommand);
            CfgOption option = TestDataBuilder.CreateCfgOption(expectedOption, expectedDataType, expectedDescription, expectedApiName);
            subCommand.Options.Add(option);


            CfgCommandList commandList = new CfgCommandList();
            commandList.Commands = new List<CfgCommand>();
            commandList.Commands.Add(command);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            builder.CfgCommands = commandList;


            GetDataHandler handler = new GetDataHandler(new NullLogger<GetDataHandler>());

            List<IOption> selectableOptions = builder.BuildAvailableOptions(handler);

            Assert.AreEqual(expectedNumberOfOptions, selectableOptions.Count);
        }

        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_One_Option_AssignValue()
        {
            string expectedOption = "TestOption";
            string expectedDataType = "string";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";
            string expectedValue = "TestValue";

            int expectedNumberOfOptions = 1;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue); 


            ISubCommandHandler subCommandHandler = new GetDataHandler(logger)
            {
                SelectableCliOptions = selectableOptions,
                DictOptions = cliOptions
            };

            bool IsValid = builder.AssignValueToCliOptions(subCommandHandler);

            Assert.IsTrue(IsValid);
            Assert.AreEqual(expectedNumberOfOptions, subCommandHandler.SelectableCliOptions.Count);
            IOption selectableoption = subCommandHandler.SelectableCliOptions[0];

            Assert.AreEqual(expectedOption, selectableoption.Name);
            Assert.AreEqual(expectedDescription, selectableoption.Description);
            Assert.AreEqual(expectedApiName, selectableoption.ApiName);
            Assert.IsTrue(selectableoption.IsAssigned);
            Assert.AreEqual(expectedValue, selectableoption.Value);

            Assert.IsInstanceOfType(selectableoption, typeof(NumberOption<String>));
        }

        [TestMethod]
        public void OptionBuilder_CreateOption_Command_Found_One_Option_AssignValue_Wrong_Input_Value()
        {
            string expectedOption = "TestOption";
            string expectedDataType = "Guid";
            string expectedDescription = "Option test description";
            string expectedApiName = "TestApiName";
            string expectedValue = "TestValue";

            int expectedNumberOfOptions = 1;

            //Build environment, 
            string envirnonmentSetting = $"{{\"UseLiveClient\": \"True\"}}";
            NullLogger<Microsoft.Extensions.Logging.ILogger> logger = new NullLogger<Microsoft.Extensions.Logging.ILogger>();

            BuildEnvironment(envirnonmentSetting, logger);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue);


            ISubCommandHandler subCommandHandler = new GetDataHandler(logger)
            {
                SelectableCliOptions = selectableOptions,
                DictOptions = cliOptions
            };

            bool isValid = builder.AssignValueToCliOptions(subCommandHandler);

            Assert.IsFalse(isValid);
            Assert.AreEqual(expectedNumberOfOptions, subCommandHandler.SelectableCliOptions.Count);
            IOption selectableoption = subCommandHandler.SelectableCliOptions[0];

            Assert.AreEqual(expectedOption, selectableoption.Name);
            Assert.AreEqual(expectedDescription, selectableoption.Description);
            Assert.AreEqual(expectedApiName, selectableoption.ApiName);
            Assert.IsFalse(selectableoption.IsAssigned);
            Assert.IsFalse(string.IsNullOrEmpty(selectableoption.ErrorMessage));
        }

        private static void BuildEnvironment(string envirnonmentSetting, NullLogger<Microsoft.Extensions.Logging.ILogger> logger)
        {
            byte[] data = Encoding.ASCII.GetBytes(envirnonmentSetting);
            MemoryStream stream = new MemoryStream(data);

            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(stream);
            IConfigurationRoot configurationRoot = configBuilder.Build();

            ApplicationManager applicationManager = new ApplicationManager(logger);
            applicationManager.SetEnvironment(configurationRoot, null);
        }
    }
}
