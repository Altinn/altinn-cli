using System.Net.Http;

namespace AltinnCLI.Services.Interfaces
{
    public interface IMaskinPortenClientWrapper
    {
        bool PostToken(FormUrlEncodedContent bearer, out string token);
    }
}
