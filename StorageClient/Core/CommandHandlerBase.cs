using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public class CommandHandlerBase
    {
        public List<KeyValuePair<string,string>> ParseOptions(string[] args)
        {
            List<KeyValuePair<string, string>> options = new List<KeyValuePair<string, string>>();

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
    }
}
