using System;
using System.Collections.Generic;
using System.Linq;

namespace Altinn.Clients.StorageClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> dict = args.Select(a => a.Split('=')).ToDictionary(a => a[0].ToLower().Substring(1), a => a.Length == 2 ? a[1] : null);

            string appId = null;
            string url = null;
            bool quit = false;

           if (dict.ContainsKey("appid") && dict.ContainsKey("url"))
            { 
                appId = dict["appid"];
                url = dict["url"];
            } 
        }
    }
}
