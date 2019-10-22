using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Core
{
    /// <summary>
    /// Base class for Application Engines
    public abstract class ApplicationEngineBase
    {
        // Command line arguments split into array of string. Space is used as split char to make the array
        public string[] Args;


        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEngineBase" /> class.
        /// </summary>
        public ApplicationEngineBase()
        {
        }
    }
}
