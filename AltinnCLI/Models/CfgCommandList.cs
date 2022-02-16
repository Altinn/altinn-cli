using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AltinnCLI.Models
{
    public class CfgCommandList
    {
        [JsonPropertyName("Commands")]
        public List<CfgCommand> Commands { get; set; }
    }
}
