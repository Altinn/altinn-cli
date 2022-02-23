using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AltinnCLI.Models
{
    public class CfgCommand
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("SubCommands")]
        public List<CfgSubCommand> SubCommands { get; set; }
    }
}
