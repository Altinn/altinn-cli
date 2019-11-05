using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public class NumberOption<T> : Option<T>
    {
        public override bool IsValid()
        {
            if (TryParse(Value) == null)
            {
                return false;
            }

            return true;
        }
    }
}
