using AltinnCLI.Core.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Singleton class the loads the command file and for each command to execute build and validate options
    /// </summary>
    public sealed class OptionBuilder
    {
        private static OptionBuilder instance = null;
        private static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionBuilder" /> class.
        /// </summary>
        /// <param name="logger">The application logger</param>
        OptionBuilder(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns or if not already created creates an instance of the Option builder 
        /// </summary>
        /// <param name="logger">the application builder</param>
        /// <returns>The singleton instance of the OptionBuilder class</returns>
        public static OptionBuilder Instance(ILogger logger)
        {

            if (instance == null)
            {
                instance = new OptionBuilder(logger);
            }

            return instance;
        }

        /// <summary>
        /// Reads the command file from disk if existent otherwise use embedded definition file
        /// </summary>
        private void LoadCommandFile()
        {
            string fileName = (ApplicationManager.ApplicationConfiguration.GetSection("CommandDefinitionFile").Get<string>());

            List<IOption> subCommandOptions = new List<IOption>();
            List<CfgOption> cfgOptions = new List<CfgOption>();
            string commandDefinitions = string.Empty;

            if (File.Exists(fileName))
            {
                commandDefinitions = File.ReadAllText(fileName);

            }
            else
            {
                // Definition file was not found use, embedded defintion file
                Assembly assembly = Assembly.GetExecutingAssembly();
                StreamReader textStreamReader = new StreamReader(assembly.GetManifestResourceStream("AltinnCLI.Commands.DefinitionFiles.Commands.json"));
                commandDefinitions = textStreamReader.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(commandDefinitions))
            {
                instance.CfgCommands = JsonConvert.DeserializeObject<CfgCommandList>(commandDefinitions);
            }

        }

        /// <summary>
        ///  Builds the paramters that can be used by a subcommand. 
        /// </summary>
        /// <param name="commandHandler">The SubCommand for which the paramters are build</param>
        /// <returns>List of subcommand paramters</returns>
        public List<IOption> BuildAvailableOptions(ISubCommandHandler commandHandler)
        {
            List<IOption> subCommandOptions = new List<IOption>();
            List<CfgOption> cfgOptions = new List<CfgOption>();

            // the defintion file shall be only be read at startup
            if (instance.CfgCommands == null)
            {
                instance.LoadCommandFile();
            }

            if (CfgCommands.Commands == null || CfgCommands.Commands.Count == 0)
            {
                _logger.LogError("There is not define any cli commands, check command definition file");
            }
            else
            {
                // find defined subcommand with options and build an option list
                CfgSubCommand cfgCubCommand = CfgCommands.Commands.FirstOrDefault(x => x.Name == commandHandler.CommandProvider)?
                                                .SubCommands.FirstOrDefault(y => y.Name == commandHandler.Name);

                if (cfgCubCommand != null && cfgCubCommand.Options != null)
                {
                    cfgOptions = cfgCubCommand.Options;

                    foreach (CfgOption cfgOption in cfgOptions)
                    {
                        IOption option = CreateOption(cfgOption);
                        if (option != null)
                        {
                            subCommandOptions.Add(CreateOption(cfgOption));
                        }
                    }
                }
            }

            return subCommandOptions;
        }

        private static IOption CreateOption(CfgOption cfgOption)
        {
            Type baseType = null;

            Type t = GetSystemType(cfgOption.DataType, out baseType);

            // create option according to defined type 
            if (t != null)
            {
                var combinedType = baseType.MakeGenericType(t);
                IOption option = (IOption)Activator.CreateInstance(combinedType);

                option.Description = cfgOption.Description;
                option.Name = cfgOption.Name;
                option.IsAssigned = false;
                option.ApiName = cfgOption.Apiname;
                return option;
            }
            else
            { 
                _logger.LogError($"The defined data type: {cfgOption.DataType} for option: {cfgOption.Name} is not valid");
                return null;
            }
        }


        /// <summary>
        /// Assigns value for the options defined as input paramters to selectable parameters.
        /// </summary>
        /// <param name="commandHandler"></param>
        /// <returns>Status of assignments which includes validation of paramters</returns>
        public bool AssignValueToCliOptions(ISubCommandHandler commandHandler)
        {
            bool isValid = true;
            foreach (IOption option in commandHandler.SelectableCliOptions)
            {
                KeyValuePair<string, string> valuePair = commandHandler.DictOptions.FirstOrDefault(x => string.Equals(x.Key, option.Name, StringComparison.OrdinalIgnoreCase));

                if (valuePair.Value != null)
                {
                    // validate according to option type
                    option.Value = valuePair.Value;
                    option.IsValid = option.Validate();
                    option.IsAssigned = true;

                    if (option.IsValid == false)
                    {
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Gets or sets the list of avilable commands that is obtained either from disk or from
        /// embedded resource file
        /// </summary>
        public CfgCommandList CfgCommands{ get; set; }


        /// <summary>
        ///  Gets the System.Type for a typefind type. Is the type specified as type in the command definition file
        /// </summary>
        /// <param name="type">The string that represents a Type</param>
        /// <param name="baseType">The base class that represents the Type</param>
        /// <returns></returns>
        public static Type GetSystemType(string type, out Type baseType)
        {
            string lowerCaseType = type.ToLower();
            string systemType = string.Empty;
            baseType = null;

            switch (lowerCaseType)
            {
                case "bool":        systemType = "System.Boolean";      baseType = typeof(NumberOption<>); break;
                case "byte":        systemType = "System.Byte";         baseType = typeof(NumberOption<>); break;
                case "sbyte":       systemType = "System.Decimal";      baseType = typeof(NumberOption<>); break;
                case "double":      systemType = "System.Double";       baseType = typeof(NumberOption<>); break;
                case "float":       systemType = "System.Single";       baseType = typeof(NumberOption<>); break;
                case "int":         systemType = "System.Int32";        baseType = typeof(NumberOption<>); break;
                case "uint":        systemType = "System.UInt32";       baseType = typeof(NumberOption<>); break;
                case "long":        systemType = "System.Int64";        baseType = typeof(NumberOption<>); break;
                case "ulong":       systemType = "System.UInt64";       baseType = typeof(NumberOption<>); break;
                case "object":      systemType = "System.Object";       baseType = typeof(NumberOption<>); break;
                case "short":       systemType = "System.Int16";        baseType = typeof(NumberOption<>); break;
                case "ushort":      systemType = "System.UInt16";       baseType = typeof(NumberOption<>); break;
                case "string":      systemType = "System.String";       baseType = typeof(NumberOption<>); break;
                case "datetime":    systemType = "System.DateTime";     baseType = typeof(NumberOption<>); break;
                case "guid":        systemType = "System.Guid";         baseType = typeof(NumberOption<>); break;
                case "file":        systemType = "System.IO.FileStream";baseType = typeof(FileOption<>);   break;
                case "thumbprint":  systemType = "System.String";       baseType = typeof(ThumbPrintOption<>); break;
            }

            return Type.GetType(systemType);
        }
    }
}
