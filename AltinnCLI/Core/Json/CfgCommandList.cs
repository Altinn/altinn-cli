using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core.Json
{
    class CfgCommandList
    {
        [JsonProperty("Commands")]
        public List<CfgCommand> Commands { get; set; }
    }
}
