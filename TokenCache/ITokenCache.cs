using System.Threading.Tasks;

namespace TokenCache
{
    public interface ITokenCache
    {
        Task<string> FetchToken(string key);
    }
}