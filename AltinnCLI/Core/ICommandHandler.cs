using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Interface that defines required method and properties for a CommandHandler
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Run(); 

        /// <summary>
        /// Name of the command handler
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Name of the service provider for which the command is implemented
        /// </summary>
        string ServiceProvider { get; }

        /// <summary>
        /// Dictionary with cli input parameters
        /// </summary>
        Dictionary<string,string> CommandParameters { get; set; }

        /// <summary>
        /// Validation status of the command hanlder. 
        /// </summary>
        bool IsValid { get; }
    }
}
