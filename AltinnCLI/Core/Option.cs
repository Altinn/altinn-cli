using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace AltinnCLI.Core
{
    public class Option<T> : IOption, IHelp
    {

        public Option(string name, string value, string apiName = null)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public object GetValue()
        {
            return TryParse(Value);
        }

        public bool IsValid()
        {
            return true;
        }
          
        public string Description { get; }

        public string Usage { get; }

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        private T TryParse(string inValue)
        {
            TypeConverter converter =
                TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromString(null,
                CultureInfo.InvariantCulture, inValue);
        }
    }
    
}


