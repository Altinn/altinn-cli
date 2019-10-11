using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Services
{
    class QuitService : IService
    {
        void IService.Run(string[] args)
        {
            Environment.Exit(0);
        }


        public string Name
        {
            get
            {
                return "Quit";
            }
        }
    }
}
