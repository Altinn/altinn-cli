using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public class CommandHandlerBase
    {
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
    }
}
