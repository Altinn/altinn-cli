using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Application
{
    class ApplicationFileClientWrapper : IApplicationClientWrapper
    {
        public string CreateApplication(string org, string app, string instanceOwnerId, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public string CreateApplication(string appId, string instanceOwnerId, StringContent content)
        {
            throw new NotImplementedException();
        }
    }
}
