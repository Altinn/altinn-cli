using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core.Json
{
    public class CfgCommand
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SubCommands")]
        public List<CfgSubCommand> SubCommands { get; set; }
    }
}
