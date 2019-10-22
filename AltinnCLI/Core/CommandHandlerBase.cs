﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Base class for Commandhandlers
    /// </summary>
    public abstract class CommandHandlerBase
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase" /> class.
        /// </summary>
        /// <param name="logger">Application logger to be used for logging</param>
        public CommandHandlerBase(ILogger<CommandHandlerBase> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Dictionary<string,string> ParseOptions(string[] args)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();

            return options;
        }

        protected virtual string BuildCommand(string[] args)
        {
            return string.Empty;
        }

        protected virtual bool RunCommand(string command)
        {
            return true;
        }

        /// <summary>
        /// Gets or set the dictionary with the command line arguments
        /// </summary>
        public Dictionary<string, string> CommandParameters { get; set; }

        /// <summary>
        /// Verifies if the command parameters contain a specific key and that it has a value
        /// </summary>
        /// <param name="key">the key to verify for existens and value</param>
        /// <returns></returns>
        protected bool HasParameterWithValue(string key)
        {
            if (CommandParameters.ContainsKey(key) && CommandParameters.GetValueOrDefault(key) != string.Empty)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies if there exists a specific command parameter
        /// </summary>
        /// <param name="key">name of the parameter to find</param>
        /// <returns></returns>
        protected bool HasParameter(string key)
        {
            if (CommandParameters.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  saves a file to disk
        /// </summary>
        /// <param name="ownerId">owner id which is used as "top" directory</param>
        /// <param name="instanceId">is used as subdirectory</param>
        /// <param name="fileName">filname to be used on the saved file</param>
        /// <param name="stream">the fiel content</param>
        protected static void SaveToFile(int ownerId, Guid instanceId, string fileName, Stream stream)
        {
            string baseFolder = (ApplicationManager.ApplicationConfiguration.GetSection("StorageOutputFolder").Get<string>());
            string fileFolder = $@"{baseFolder}\{ownerId}\{instanceId}";

            // chekc if file folder exists, if not create it
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);

            }

            string filePath = $@"{fileFolder}\{fileName}";
            FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

            stream.Position = 0;
            ((MemoryStream)stream).WriteTo(file);
            file.Close();
            stream.Close();

            _logger.LogInformation($"Data for OwnerId:{ownerId} InstanceId:{instanceId} saved to file:{fileName}");
        }
    }
}
