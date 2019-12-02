using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Login
{
    public class MaskinPortenClientFileWrapper : IMaskinPortenClientWrapper
    {
        public bool PostToken(FormUrlEncodedContent bearer, out string token)
        {
            throw new NotImplementedException();
        }
    }
}
