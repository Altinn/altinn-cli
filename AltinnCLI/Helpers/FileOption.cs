using AltinnCLI.Commands.Core;
using System.IO;

namespace AltinnCLI.Helpers
{
    public class FileOption<FileStream> : Option<FileStream>
    {
        /// <summary>
        /// Verifies if the input parameters are valid.
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            if (File.Exists(Value))
            {
                return true;
            }

            ErrorMessage = $"Invalid value for parameter: {Name}, file: {Value} was not found";
            return false;
        }
    }
}
