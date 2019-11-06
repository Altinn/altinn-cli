﻿using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Base class for Commandhandlers
    /// </summary>
    public abstract class SubCommandHandlerBase
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase" /> class.
        /// </summary>
        /// <param name="logger">Application logger to be used for logging</param>
        public SubCommandHandlerBase(ILogger<SubCommandHandlerBase> logger)
        {
            _logger = logger;
            CliOptions = new List<IOption>();
        }

        /// <summary>
        /// Gets or set the dictionary with the command line arguments
        /// </summary>
        public Dictionary<string, string> DictOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> SelectableCliOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> CliOptions { get; set; }

        /// <summary>
        /// Verifies if the command parameters contain a specific key and that it has a value
        /// </summary>
        /// <param name="key">the key to verify for existens and value</param>
        /// <returns></returns>
        protected bool HasParameterWithValue(string key)
        {
            if (DictOptions.ContainsKey(key) && DictOptions.GetValueOrDefault(key) != string.Empty)
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
            if (DictOptions.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected object GetOptionValue(string key)
        {
            return SelectableCliOptions.FirstOrDefault(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase) && x.IsAssigned)?.GetValue();
        }

        /// <summary>
        ///  saves a file to disk
        /// </summary>
        /// <param name="filePath">the path excluded a base path for where the file shall be saved</param>
        /// <param name="fileName">filname to be used on the saved file</param>
        /// <param name="stream">the fiel content</param>
        protected void SaveToFile(string filePath, string fileName, Stream stream)
        {
            string baseFolder = (ApplicationManager.ApplicationConfiguration.GetSection("StorageOutputFolder").Get<string>());
            string fileFolder = $@"{baseFolder}\{filePath}";

            // chekc if file folder exists, if not create it
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);

            }

            using (FileStream outputFile = new FileStream(Path.Combine(fileFolder, fileName), FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.CopyTo(outputFile);
            }

            _logger.LogInformation($"File:{fileName} saved at {fileFolder}");
        }
    }
}
