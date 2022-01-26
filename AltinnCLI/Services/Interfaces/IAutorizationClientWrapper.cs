using System.Threading.Tasks;

namespace AltinnCLI.Services.Interfaces
{
    public interface IAutorizationClientWrapper
    {
        Task<string> ConvertToken(string token, bool test = false);
    }
}
