using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Login
{
    public interface IMaskinPortenClientWrapper
    {
        bool PostToken(FormUrlEncodedContent bearer, out string token); 
    }
}
