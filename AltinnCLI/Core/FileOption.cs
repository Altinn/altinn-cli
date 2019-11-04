using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace AltinnCLI.Core
{
    public class FileOption<T>  : Option<T> where T : FileStream
    {
        /// <summary>
        /// Verifies if the input parameters are valid.
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            if (File.Exists(Value))
            {
                return true;
            }
            return false;
        }

        protected override T TryParse(string inValue)
        {

            if (File.Exists(inValue))
            {
                return (T)default(FileStream); ///; (FileStream)File.Open(inValue, FileMode.Open);
            }

            return (T)default(FileStream);
        }
    }
}
