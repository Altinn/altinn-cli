using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Core
{
    public abstract class ApplicationEngineBase
    {
        public string[] Args;



        public ApplicationEngineBase()
        {
        }
    }
}
