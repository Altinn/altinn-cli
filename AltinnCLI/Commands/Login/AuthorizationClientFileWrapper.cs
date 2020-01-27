using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Commands.Login
{
    class AuthorizationClientFileWrapper : IAutorizationClientWrapper
    {
        public Task<string> ConvertToken(string toke, bool test = false)
        {
            throw new NotImplementedException();
        }
    }
}
