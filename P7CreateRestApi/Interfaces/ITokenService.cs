using P7CreateRestApi.Models;

namespace P7CreateRestApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
