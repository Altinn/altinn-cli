using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IValidate
    {
        public bool Validate();

        public bool IsValid { get; set; }

        public string ErrorMessage { get; set; }

    }
}
