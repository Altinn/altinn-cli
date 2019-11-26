using AltinnCLI;
using AltinnCLI.Core;
using AltinnCLI.Core.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLITest
{
    public static class TestDataBuilder
    {
        public static CfgCommand CreateCfgCommand(string commandName)
        {
            CfgCommand command = new CfgCommand()
            {
                Name = commandName,
                SubCommands = new List<CfgSubCommand>()
            };

            return command;
        }

        public static CfgSubCommand CreateCfgSubCommand(string subCommandName)
        {
            CfgSubCommand subCommand = new CfgSubCommand()
            {
                Name = subCommandName,
                Options = new List<CfgOption>()
            };

            return subCommand;
        }

        public static CfgOption CreateCfgOption(string name, string dataType, string description, string apiName)
        {
            CfgOption option = new CfgOption()
            {
                Name = name,
                DataType = dataType,
                Description = description,
                Apiname = apiName
            };

            return option;
        }

        public static IOption CreateOption(string optionName, string dataType, string description, string apiName)
        {
            Type baseType = null;

            Type t = OptionBuilder.GetSystemType(dataType, out baseType);

            var combinedType = baseType.MakeGenericType(t);
            IOption option = (IOption)Activator.CreateInstance(combinedType);

            option.Description = description;
            option.Name = optionName;
            option.IsAssigned = false;
            option.ApiName = apiName;
            return option;
        }


        public static ServiceProvider BuildServiceProvider(List<Type> availableCommandTypes, List<Type> availableSubCommands, Serilog.ILogger logger)
        {


            IServiceCollection services = new ServiceCollection();

            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddProvider(new SerilogLoggerProvider(logger));
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
                    configure.AddProvider(new SerilogLoggerProvider(logger));
                }).AddTransient(typeof(ISubCommandHandler), t);
            });

            ServiceProvider provider = services.BuildServiceProvider();

            return provider;
        }
    }
}
