﻿using System.Collections.Generic;

using Newtonsoft.Json;

namespace AltinnCLI.Core.Json
{
    public class CfgCommandList
    {
        [JsonProperty("Commands")]
        public List<CfgCommand> Commands { get; set; }
    }
}
