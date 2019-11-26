using AltinnCLI;
using AltinnCLI.Commands.Storage;
using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class SubCommandHandlerTest
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

            TextWriter textWriter = new StringWriter();
            ConfigureLogging(textWriter);
            
            BuildEnvironment(envirnonmentSetting);

            OptionBuilder builder = OptionBuilder.Instance(logger);
            List<IOption> selectableOptions = new List<IOption>();
            selectableOptions.Add(TestDataBuilder.CreateOption(expectedOption, expectedDataType, expectedDescription, expectedApiName));
            Dictionary<string, string> cliOptions = new Dictionary<string, string>();
            cliOptions.Add(expectedOption, expectedValue);

            List<Type> availableCommandTypes = new List<Type>();
            availableCommandTypes.Add(typeof(StorageCommand));

            List<Type> availableSubCommands = new List<Type>();
            availableSubCommands.Add(typeof(GetDataHandler));

            ServiceProvider serviceProvider = TestDataBuilder.BuildServiceProvider(availableCommandTypes, availableSubCommands, Log.Logger);


            ISubCommandHandler commandHandler = serviceProvider.GetServices<ISubCommandHandler>()
             .FirstOrDefault(s => string.Equals(s.Name, "GetData", StringComparison.OrdinalIgnoreCase));



            ISubCommandHandler subCommandHandler = serviceProvider.GetService<GetDataHandler>();
            subCommandHandler.SelectableCliOptions = selectableOptions;
            subCommandHandler.DictOptions = cliOptions;

            builder.AssignValueToCliOptions(subCommandHandler);

            subCommandHandler.Run();


            List<string> logEntries = GetLogEntries(textWriter);
            Assert.AreEqual(expectedLogEntires, logEntries.Count);
            string logMessage = logEntries.FirstOrDefault(x => x.Contains(expectedLogMessage));

            Assert.IsFalse(string.IsNullOrEmpty(logMessage));

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
