using AltinnCLI.Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AltinnCLI.Commands
{
    class LoginCommand : ICommand, IHelp
    {
        private IServiceProvider ServiceProvider;

        /// <summary>
        /// 
        /// </summary>
        public LoginCommand()
        {
            ServiceProvider = ApplicationManager.ServiceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return "Login";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                return $"\tVerifiy certificates and log in to Altinn";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Usage
        {
            get
            {
                return $@"Login cert=<filepath> password=<password>";
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

        public void Run(ISubCommandHandler commandHandler = null)
        {
            Console.WriteLine("Logged in");
        }

        public void Run(Dictionary<string, string> input)
        {
            throw new NotImplementedException();
        }

        private X509Certificate readCertificate(string filepath)
        {
            return new X509Certificate2(filepath, string.Empty, X509KeyStorageFlags.PersistKeySet);
        }
    }
}
