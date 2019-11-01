using AltinnCLI.Core.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AltinnCLI.Core
{
    public sealed class OptionBuilder
    {
        private static OptionBuilder instance = null;
        private static readonly object padlock = new object();
        private static CfgCommandList cfgCommands = null;

        /// <summary>
        /// Application logger 
        /// </summary>
        private static ILogger _logger;

        OptionBuilder(ILogger logger)
        {
        }

        public static OptionBuilder Instance(ILogger logger)
        {
            //get
            //{
            //    lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new OptionBuilder(logger);
                        
                        LoadCommandFile();
                    }
                }
                return instance;
 //           }
        }

        private static void LoadCommandFile()
        {
            string fileName = (ApplicationManager.ApplicationConfiguration.GetSection("CommandDefinitionFile").Get<string>());

            List<IOption> subCommandOptions = new List<IOption>();
            List<CfgOption> cfgOptions = new List<CfgOption>();

            if (File.Exists(fileName))
            {
                string commandDefinitions = File.ReadAllText(fileName);

                if (!string.IsNullOrEmpty(commandDefinitions))
                {
                    cfgCommands = JsonConvert.DeserializeObject<CfgCommandList>(commandDefinitions);
                }
            }
        }

        public  List<IOption> BuildAvailableOptions(ISubCommandHandler commandHandler)
        {
            string fileName = (ApplicationManager.ApplicationConfiguration.GetSection("CommandDefinitionFile").Get<string>());

            List<IOption> subCommandOptions = new List<IOption>();
            List<CfgOption> cfgOptions = new List<CfgOption>();

            if (File.Exists(fileName))
            {
                string commandDefinitions = File.ReadAllText(fileName);

                if (!string.IsNullOrEmpty(commandDefinitions))
                {
                    CfgCommandList cfgCommands = JsonConvert.DeserializeObject<CfgCommandList>(commandDefinitions);

                    CgSubCommand cfgCubCommand = cfgCommands.Commands.FirstOrDefault(x => x.Name == commandHandler.CommandProvider)?
                                                    .SubCommands.FirstOrDefault(y => y.Name == commandHandler.Name);

                    if ( cfgCubCommand != null && cfgCubCommand.Options != null)
                    {
                        cfgOptions = cfgCubCommand.Options;

                        foreach (CfgOption cfgOption in cfgOptions)
                        {
                            Type t = GetSystemType(cfgOption.DataType);

                            var baseClass = typeof(Option<>);
                            var combinedType = baseClass.MakeGenericType(t);
                            IOption option = (IOption)Activator.CreateInstance(combinedType);

                            option.Description = cfgOption.Description;
                            option.Name = cfgOption.Name;
                            option.IsAssigned = false;

                            subCommandOptions.Add(option);
                        }
                    }
                }
            }

            return subCommandOptions;
        }


        public  void AssignValueToCliOptions(ISubCommandHandler commandHandler)
        {
 
            foreach (Option<IOption> option in commandHandler.SelectableCliOptions)
            {
                KeyValuePair<string, string> valuePair = commandHandler.DictOptions.FirstOrDefault(x => x.Key == option.Name);

                if (valuePair.Value != null)
                {
                    option.Value = valuePair.Value;
                    if (option.IsValid())
                    {
                        option.IsAssigned = true;
                    }
                }
            }
        }



        private static Type GetSystemType(string type)
        {
            string lowerCaseType = type.ToLower();

            string systemType = string.Empty;

            switch (lowerCaseType)
            {
                case "bool": systemType = "System.Boolean"; break;
                case "byte": systemType = "System.Byte"; break;
                case "sbyte": systemType = "System.Decimal"; break;
                case "double": systemType = "System.Double"; break;
                case "float": systemType = "System.Single"; break;
                case "int": systemType = "System.Int32"; break;
                case "uint": systemType = "System.UInt32"; break;
                case "long": systemType = "System.Int64"; break;
                case "ulong": systemType = "System.UInt64"; break;
                case "object": systemType = "System.Object"; break;
                case "short": systemType = "System.Int16"; break;
                case "ushort": systemType = "System.UInt16"; break;
                case "string": systemType = "System.String"; break;
                case "datetime": systemType = "System.DateTime"; break;
            }


            return Type.GetType(systemType);
        }
    }
}
