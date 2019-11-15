using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Login
{
    public interface IMaskinPortenClientWrapper
    {
        string PostToken(FormUrlEncodedContent bearer); 
    }
}
