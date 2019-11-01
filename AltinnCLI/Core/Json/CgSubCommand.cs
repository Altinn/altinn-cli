using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core.Json
{
    public class CgSubCommand
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Options")]
        public List<CfgOption> Options { get; set; }
    }
}
