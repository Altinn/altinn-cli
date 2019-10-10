using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
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
