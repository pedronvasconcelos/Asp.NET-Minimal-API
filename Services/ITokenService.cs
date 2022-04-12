using CatalogAPI.Models;

namespace CatalogAPI.Services
{
    public interface ITokenService
    {
        string GenerateToken(string key, string issuer, string audience, UserModel user);
    }
}
