using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCLI.Commands.Login
{
    public interface IAutorizationClientWrapper
    {
        Task<string> ConvertToken(string token, bool test=false);
    }
}
