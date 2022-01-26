using System.Collections.Generic;

using Newtonsoft.Json;

namespace AltinnCLI.Models
{
    public class CfgCommand
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SubCommands")]
        public List<CfgSubCommand> SubCommands { get; set; }
    }
}
