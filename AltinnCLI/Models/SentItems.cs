using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AltinnCLI.Models
{
    public class SentItems
    {
        [JsonPropertyName("reference")]
        public List<string> Reference;
    }
}
