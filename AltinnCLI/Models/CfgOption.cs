using System.Text.Json.Serialization;

namespace AltinnCLI.Models
{
    public class CfgOption
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string DataType { get; set; }

        [JsonPropertyName("valuerangeange")]
        public string Valuerangeange { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("apiname")]
        public string Apiname { get; set; }
    }
}
