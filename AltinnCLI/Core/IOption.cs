using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IOption : IValidate
    {
        public string Name { get; set;  }

        public string ApiName { get; set; }

        string Value { get; set; }

        bool IsAssigned { get; set; }

        string Description { get; set; }

        string Range { get; set; }

        object GetValue();

    }
}
