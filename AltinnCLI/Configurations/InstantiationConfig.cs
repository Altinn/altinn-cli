using System.Collections.Generic;

namespace AltinnCLI.Configurations
{
    public class InstantiationConfig
    {
        public Dictionary<string, string> ApplicationIdLookup { get; set; } = new Dictionary<string, string>();
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string ErrorFolder { get; set; }

    }
}
