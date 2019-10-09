using Microsoft.Extensions.Configuration;
using StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace StorageClient
{
    class Program
    {
        static void Main()
        {

            ApplicationManager app = new ApplicationManager();
            string args;            

            while(true)
            {
                Console.Write(prompt);
                args = Console.ReadLine();
                app.Execute(args);
            }
        }

        private static string prompt = "Altinn CLI > ";
    }
}
