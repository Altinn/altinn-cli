using System;
using System.Collections.Generic;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Interface that defines required method and properties for a cli command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Run the supported command handler
        /// </summary>
        /// <param name="commandHandler">the command handler to execute</param>
        void Run(ISubCommandHandler commandHandler = null);

        /// <summary>
        /// Parses the dictionary and run command. Used mainly by Help
        /// </summary>
        /// <param name="input">Dictionary with the cli input paramters</param>
        void Run(Dictionary<string, string> input);

        /// <summary>
        /// Gets the name of the service
        /// </summary>
        string Name { get; }
    }
}
