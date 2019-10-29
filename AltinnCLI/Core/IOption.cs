using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IOption
    {
        object GetValue();

        bool IsValid();
    }
}
