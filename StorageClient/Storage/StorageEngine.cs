using AltinnCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public class StorageEngine : ApplicationEngineBase, IService, IHelp
    {
        public StorageEngine()
        {
        }
        public void Run(string[] args)
        {
            BuildConfiguration();
        }

        public override void BuildConfiguration()
        {
            base.BuildConfiguration();
            string baseAddresspath = ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            bool useLiveClient = ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>();
        }

        public string GetHelp()
        {
            return "Storage help";
        }
    }

}
