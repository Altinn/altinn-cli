using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageClient
{
    public class StorageEngine : ApplicationEngineBase
    {
        public StorageEngine()
        {
        }

        public override void BuildConfiguration()
        {
            string baseAddresspath = ApplicationConfiguration.GetSection("APIBaseAddress").Get<string>();
            bool useLiveClinet = ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>();
        }
    }

}
