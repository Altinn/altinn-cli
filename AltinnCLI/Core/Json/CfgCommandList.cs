using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core.Json
{
    public class CfgCommandList
    {
        [JsonProperty("Commands")]
        public List<CfgCommand> Commands { get; set; }
    }
}
