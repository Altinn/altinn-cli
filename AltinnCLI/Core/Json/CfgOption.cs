using Newtonsoft.Json;

namespace AltinnCLI.Core.Json
{
    public class CfgOption
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string DataType { get; set; }

        [JsonProperty("valuerangeange")]
        public string Valuerangeange { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("apiname")]
        public string Apiname { get; set; }
    }
}
