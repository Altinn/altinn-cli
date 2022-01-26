using AltinnCLI.Services.Interfaces;
using System;
using System.Net.Http;

namespace AltinnCLI.Services
{
    public class MaskinPortenClientFileWrapper : IMaskinPortenClientWrapper
    {
        public bool PostToken(FormUrlEncodedContent bearer, out string token)
        {
            throw new NotImplementedException();
        }
    }
}
