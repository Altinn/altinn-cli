using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IOption
    {
        public string Name { get; set;  }

        public string ApiName { get; set; }

        string Value { get; set; }

        bool IsAssigned { get; set; }

        string Description { get; set; }

        object GetValue();

        bool IsValid();

        bool HasValue();
    }
}
