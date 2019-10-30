using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace AltinnCLI.Core
{
    public class Option<T> : IOption, IHelp
    {

        /// <summary>
        /// Creates an instance of the <see cref="Option" /> class
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="value">Value given for the option. Read from the command line</param>
        /// <param name="apiName">Mapping to internal API representatio</param>
        public Option(string name, string value, string apiName = null)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the option
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the option. Read from the command line
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Function to get the value of the parameter
        /// </summary>
        /// <returns>The value given to the option parsed into the correct type for the option, default value for the type if the value can not be parsed</returns>
        public object GetValue()
        {
            return TryParse(Value);
        }

        /// <summary>
        /// Function to verify that the option is valid
        /// </summary>
        /// <returns>True if the option is given a legal value, false otherwise</returns>
        public bool IsValid()
        {
            return true;
        }
          
        /// <summary>
        /// Gets the description for the option.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the usage examples for the option
        /// </summary>
        public string Usage { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return $@"{Name} - \t\t{Description}";
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


