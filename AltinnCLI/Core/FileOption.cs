using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace AltinnCLI.Core
{
    public class FileOption<FileStream>  : Option<FileStream>
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
            return false;
        }
    }
}
