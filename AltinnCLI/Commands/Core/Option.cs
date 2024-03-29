﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace AltinnCLI.Commands.Core
{
    public abstract class Option<T> : IOption //, IHelp
    {
        protected bool? isValid = null;

        /// <summary>
        /// Creates an instance of the <see cref="Option"/> class. Is required for building instance with correct Type
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="apiName">Mapping to internal API representatio</param>
        public Option()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="Option" /> class
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="apiName">Mapping to internal API representatio</param>
        public Option(string name, string apiName = null)
        {
            Name = name;
            ApiName = apiName;

        }

        /// <summary>
        /// Gets the name of the option
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the value of the option. Read from the command line
        /// </summary>
        public string Value { get; set; }

        public string ApiName { get; set; }

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
        public virtual bool IsValid
        {
            get
            {
                if (!isValid.HasValue)
                {
                    isValid = Validate();
                }

                return (bool)isValid;
            }

            set
            {
                isValid = value;
            }
        }

        /// <summary>
        /// Gets the description for the option.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the usage examples for the option
        /// </summary>
        public string Usage { get; }

        public bool IsAssigned { get; set; }

        public string ErrorMessage { get; set; }

        public string Range { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return $@"{Name} - \t\t{Description}";
        }

        protected virtual T TryParse(string inValue)
        {
            TypeConverter converter =
                TypeDescriptor.GetConverter(typeof(T));

            try
            {
                var value = (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
                IsValid = true;
                return value;
            }
            catch (Exception ex)
            {
                IsValid = false;
                ErrorMessage = $"Invalid Parameter value for paramter: <{Name}>,  {ex.Message}";
                throw;
            }
        }

        protected virtual bool CheckRange()
        {
            return true;
        }

        public abstract bool Validate();
    }

}


