using AltinnCLI.Core;
using AltinnCLI.Core.Json;
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
    }
}
