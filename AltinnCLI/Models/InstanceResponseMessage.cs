using Altinn.Platform.Storage.Interface.Models;
using System;
using System.Text.Json.Serialization;

namespace AltinnCLI.Models
{
    /// <summary>
    /// Entity that represents the respons on a Instance request
    /// </summary>
    public class InstanceResponseMessage
    {
        /// <summary>
        /// Gets or sets the total number of instances found
        /// </summary>
        [JsonPropertyName("totalHits")]
        public long TotalHits { get; set; }

        /// <summary>
        /// Gets or sets the number of instances in "this" response
        /// </summary>
        [JsonPropertyName("count")]
        public long Count { get; set; }

        /// <summary>
        /// URL to fetch next page with instances
        /// </summary>
        [JsonPropertyName("next")]
        public Uri Next { get; set; }

        /// <summary>
        /// Gets or sets the Self link
        /// </summary>
        [JsonPropertyName("self")]
        public Uri Self { get; set; }

        /// <summary>
        /// Gets or sets the instances
        /// </summary>
        [JsonPropertyName("instances")]
        public Instance[] Instances { get; set; }
    }
}
