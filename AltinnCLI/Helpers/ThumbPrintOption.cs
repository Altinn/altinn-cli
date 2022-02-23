using AltinnCLI.Commands.Core;
using System;
using System.Security.Cryptography.X509Certificates;

namespace AltinnCLI.Helpers
{
    public class ThumbPrintOption<String> : Option<string>
    {
        /// <summary>
        /// Verifies if the input parameters are valid.
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates;

            foreach (var certificate in certificates)
            {
                if (string.Equals(certificate.Thumbprint, Value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            ErrorMessage = $"No ceriticate was found for the thumbprint:{Value}";
            return false;
        }

    }
}
