using AltinnCli;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StorageClient
{
    public class ApplicationManager
    {

        public static IConfigurationRoot ApplicationConfiguration;
        public static IServiceProvider ServiceProvider;

        public ApplicationManager(IServiceProvider serviceProvider, IConfigurationRoot applicationConfiguration)
        {
            ServiceProvider = serviceProvider;
            ApplicationConfiguration = applicationConfiguration;
        }


        public void Execute(string args)
        {
            string[] input = processArgs(args);

            ServiceProvider.GetServices<IService>().Where(s => string.Equals(s.Name, input[0], StringComparison.OrdinalIgnoreCase)).Single().Run(input);
        }

        private string[] processArgs(string args)
        {
            return args.ToLower().Split(" ");
        }
    }
}
