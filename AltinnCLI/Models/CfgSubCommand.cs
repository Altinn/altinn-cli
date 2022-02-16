using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AltinnCLI.Models
{
    public class CfgSubCommand
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Options")]
        public List<CfgOption> Options { get; set; }
    }
}
