﻿using AltinnCLI.Helpers;
using System.Collections.Generic;

namespace AltinnCLI.Commands.Core
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
        Dictionary<string, string> DictOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IOption> SelectableCliOptions { get; set; }

        /// <summary>
        /// Dictionary with cli input options
        /// </summary>
        List<IOption> CliOptions { get; set; }

        /// <summary>
        /// Dictionary with cli input options
        /// </summary>
        IFileWrapper CliFileWrapper { get; set; }

        /// <summary>
        /// Builds the options that can control the command.
        /// </summary>
        void BuildSelectableCommands();
    }
}
