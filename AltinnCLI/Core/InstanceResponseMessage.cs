using System;

using Altinn.Platform.Storage.Interface.Models;

using Newtonsoft.Json;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Entity that represents the respons on a Instance request
    /// </summary>
    public class InstanceResponseMessage
    {
        /// <summary>
        /// Gets or sets the total number of instances found
        /// </summary>
        [JsonProperty("totalHits")]
        public long TotalHits { get; set; }

        /// <summary>
        /// Gets or sets the number of instances in "this" response
        /// </summary>
        [JsonProperty("count")]
        public long Count { get; set; }

        /// <summary>
        /// URL to fetch next page with instances
        /// </summary>
        [JsonProperty("next")]
        public Uri Next { get; set; }

        /// <summary>
        /// Gets or sets the Self link
        /// </summary>
        [JsonProperty("self")]
        public Uri Self { get; set; }

        /// <summary>
        /// Gets or sets the instances
        /// </summary>
        [JsonProperty("instances")]
        public Instance[] Instances { get; set; }
    }
}
