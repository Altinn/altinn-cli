using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Interface that defines required method and properties for a SubCommandHandler
    /// </summary>
    public interface ISubCommandHandler : IValidate
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
        /// Name of the command for which the command is implemented
        /// </summary>
        string CommandProvider { get; }

        /// <summary>
        /// Dictionary with cli input options
        /// </summary>
        Dictionary<string,string> DictOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> SelectableCliOptions { get; set; }

        /// <summary>
        /// Dictionary with cli input options
        /// </summary>
        List<IOption> CliOptions { get; set; }

    }
}
