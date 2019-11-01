using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace AltinnCLI.Core
{
    public class Option<T> : IOption //, IHelp
    {

        public Option()
        {
            IsAssigned = false;
        }

        public Option(string name, string apiName = null)
        {
            Name = name;
            ApiName = apiName;
        }

        public Option(string name, string value = null, string apiName = null) : this(value, apiName)
        {
            Name = name;
            ApiName = apiName;

            if (value != null)
            {
                Value = value;
            }
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string ApiName { get; set; }

        public object GetValue()
        {
            return TryParse(Value);
        }

        public bool IsValid()
        {
            if (TryParse(Value) == null)
            {
                return false;
            }

            return true;
        }
          
        public string Description { get; set; }

        public string Usage { get; }
        public bool IsAssigned { get; set; }

        public string GetHelp()
        {
            return "Not implemented help for Option";
        }

        private T TryParse(string inValue)
        {
            TypeConverter converter =
                TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromString(null,
                CultureInfo.InvariantCulture, inValue);
        }

        public bool HasValue()
        {
            throw new NotImplementedException();
        }
    }
    
}


