using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Microsoft.Extensions.Logging.Abstractions;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Base class for Commandhandlers
    /// </summary>
    public abstract class SubCommandHandlerBase : IValidate
    {
        private string errorMessage;
        private bool? isValid;

        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase" /> class.
        /// </summary>
        /// <param name="logger">Application logger to be used for logging</param>
        public SubCommandHandlerBase(ILogger<SubCommandHandlerBase> logger) : this(new FileWrapper(logger))
        {
            _logger = logger;
            isValid = null;
            CliOptions = new List<IOption>();
        }

        public SubCommandHandlerBase(FileWrapper fileWrapper)
        {
            CliFileWrapper = fileWrapper;
        }

        /// <summary>
        /// Gets or set the dictionary with the command line arguments
        /// </summary>
        public Dictionary<string, string> DictOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> SelectableCliOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> CliOptions { get; set; }

        /// <summary>
        /// Gets or sets the file wrapper 
        /// </summary>
        public IFileWrapper CliFileWrapper { get; set; }

        /// <summary>
        /// Verifies if the command parameters contain a specific key and that it has a value
        /// </summary>
        /// <param name="key">the key to verify for existens and value</param>
        /// <returns></returns>
        protected bool HasParameterWithValue(string key)
        {
            if (DictOptions.ContainsKey(key) && DictOptions.GetValueOrDefault(key) != string.Empty)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies if there exists a specific command parameter
        /// </summary>
        /// <param name="key">name of the parameter to find</param>
        /// <returns></returns>
        protected bool HasParameter(string key)
        {
            if (DictOptions.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected object GetOptionValue(string key)
        {
            return SelectableCliOptions.FirstOrDefault(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase) && x.IsAssigned)?.GetValue();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected IOption GetOption(string key)
        {
            return SelectableCliOptions.FirstOrDefault(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
        }

         public void BuildSelectableCommands()
        {
            SelectableCliOptions = OptionBuilder.Instance(_logger).BuildAvailableOptions((ISubCommandHandler)this);
        }


        public virtual bool Validate()
        {
            // validate all options, not valid if one fails
            List<IOption> opt = SelectableCliOptions.Where(x => x.IsValid == false && x.IsAssigned == true).ToList();

            if (opt == null || opt.Count == 0)
            {
                return true;
            }

            return false;
        }


        public string GetParameterErrors()
        {
            string errorMessage = string.Empty;
            List<IOption> optionsWithError = SelectableCliOptions.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).ToList();
            int i = 0;
            foreach (IOption option in optionsWithError)
            {
                if (!option.IsValid)
                {
                    errorMessage += $"{option.ErrorMessage}";
                }
                errorMessage += (i < optionsWithError.Count - 1) ? errorMessage += $"\n" : string.Empty;
            }

            return errorMessage;
        }


        public bool IsValid
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

        public string ErrorMessage
        {
            get 
            {
                string message = $"{errorMessage} \n";

                List<IOption> optionsWithError = SelectableCliOptions.Where(x => (x.IsAssigned == true) && (x.IsValid == false)).ToList();
                foreach (IOption option in optionsWithError)
                {
                    message += $"{option.ErrorMessage} \n";
                }

                return message;
            }

            set
            {
                errorMessage = value;
            }
        }
    }
}
