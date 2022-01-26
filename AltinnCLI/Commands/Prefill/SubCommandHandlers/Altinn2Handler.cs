using AltinnCLI.Commands.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Commands.Prefill.SubCommandHandlers
{
    public class Altinn2Handler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {
        public Altinn2Handler(ILogger<Altinn2Handler> logger) : base(logger)
        {
        }

        /// <summary>
        /// Gets the name of the command
        /// </summary>
        public string Name
        {
            get
            {
                return "Altinn2";
            }
        }

        /// <summary>
        /// Gets the description of the command. Will be used to generate help documentation
        /// </summary>
        public string Description
        {
            get
            {
                return "Create Altinn 3 instances from Altinn 2 prefill-data";
            }
        }

        /// <summary>
        /// Gets the usage of the command. Will be used to generate help documentation
        /// </summary>
        public string Usage
        {
            get
            {
                return $"AltinnCLI > Prefill altinn2 \n\n";
            }
        }

        /// <summary>
        /// Gets the name of the CommandProvider for the command
        /// </summary>
        public string CommandProvider
        {
            get
            {
                return "Prefill";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes and runs the command
        /// </summary>
        /// <returns>Returns true if the command completes succesfully</returns>
        public bool Run()
        {
            return true;
        }
    }
}
