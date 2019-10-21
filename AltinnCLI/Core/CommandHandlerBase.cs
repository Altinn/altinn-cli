using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Core
{
    public abstract class CommandHandlerBase
    {
        protected static ILogger _logger;

        public CommandHandlerBase(ILogger<CommandHandlerBase> logger)
        {
            _logger = logger;
        }

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

        public Dictionary<string, string> CommandParameters { get; set; }

        protected bool HasParameterWithValue(string key)
        {
            if (CommandParameters.ContainsKey(key) && CommandParameters.GetValueOrDefault(key) != string.Empty)
            {
                return true;
            }

            return false;
        }

        protected bool HasParameter(string key)
        {
            if (CommandParameters.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        protected bool HasOptionalParameterWithIncorrectValue(string key)
        {
            if (CommandParameters.ContainsKey(key) && CommandParameters.GetValueOrDefault(key) != string.Empty)
            {
                if (!(CommandParameters.GetValueOrDefault(key) != string.Empty))
                {
                    return false;
                }
            }

            return true;
        }


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
