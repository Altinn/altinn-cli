using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Interface that defines all requirments to ApplicationEngine classes
    /// </summary>
    public interface IApplicationEngine
    {
        /// <summary>
        /// Execute the user command with parameters. Consists in finding correct service and command handler and start service.
        /// </summary>
        /// <param name="args">input command line arguments</param>
        void Execute(string[] args);


        string Name { get; }
    }
}
