using Microsoft.Extensions.Configuration;
using StorageClient;
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
            ApplicationManager  manager = new ApplicationManager(args);

            Console.WriteLine("Application completed");
        }
    }
}
