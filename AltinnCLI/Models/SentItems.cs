using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Models
{
    public class SentItems
    {
        [JsonProperty("reference")]
        public List<string> Reference;
    }
}
