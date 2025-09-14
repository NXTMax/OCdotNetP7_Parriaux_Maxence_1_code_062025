using P7CreateRestApi.Models;

namespace P7CreateRestApi.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user);
    }
}
