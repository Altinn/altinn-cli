using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnCLI.Commands.Login
{
    public interface IAutorizationClientWrapper
    {
        string ConvertToken(string token);
    }
}
