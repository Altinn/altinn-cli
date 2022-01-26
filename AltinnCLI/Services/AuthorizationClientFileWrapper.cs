using AltinnCLI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace AltinnCLI.Services
{
    class AuthorizationClientFileWrapper : IAutorizationClientWrapper
    {
        public Task<string> ConvertToken(string toke, bool test = false)
        {
            throw new NotImplementedException();
        }
    }
}
