using Microsoft.Extensions.Configuration;
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

            Console.WriteLine(prompt);
            string args = Console.ReadLine();

            BuildConfiguration();

            bool quit = false;

            while(!quit)
            {
                ApplicationManager app = new ApplicationManager(args);
            }
        }

        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");


            IConfigurationRoot configuration = builder.Build();
            string baseAddresspath = configuration.GetSection("APIBaseAddress").Get<string>();
            bool useLiveClinet = configuration.GetSection("UseLiveClient").Get<bool>();
        }

        private static string prompt = "Altinn CLI > ";
    }
}
