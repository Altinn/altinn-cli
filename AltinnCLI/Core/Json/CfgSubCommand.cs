using System.Collections.Generic;

using Newtonsoft.Json;

namespace AltinnCLI.Core.Json
{
    public class CfgSubCommand
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Options")]
        public List<CfgOption> Options { get; set; }
    }
}
