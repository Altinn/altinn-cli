using Altinn.Platform.Storage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public class InstanceResponseMessage
    {
        [JsonProperty("totalHits")]
        public long TotalHits { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("next")]
        public Uri Next { get; set; }

        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("instances")]
        public Instance[] Instances { get; set; }
    }
}
