using get5_web.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace get5_web.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<User?> CreateUserWithLogin(string username, ExternalLoginInfo info);
    }
}
