using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Altinn.Clients.StorageClient
{
    class Program
    {
        static void Main(string[] args)
        {
            StorageClient storageClinet = new StorageClient();

            BuildConfiguration();

            Console.WriteLine("Hello World!");
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
    }
}
