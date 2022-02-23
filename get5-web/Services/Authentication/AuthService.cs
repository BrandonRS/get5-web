using get5_web.Interfaces.Authentication;
using get5_web.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace get5_web.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private ILogger<AuthService> _logger;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public AuthService(ILogger<AuthService> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
        }

        public async Task<User?> CreateUserWithLogin(string username, ExternalLoginInfo info)
        {
            _logger.LogInformation("Creating user with username '{username}' for external login '{provider}'.",
                username, info.ProviderDisplayName);

            var newUser = new User { UserName = username };
            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully created user '{username}'.", username);
                result = await _userManager.AddLoginAsync(newUser, info);

                if (result.Succeeded)
                    _logger.LogInformation("Successfully added login '{provider}' for user '{username}'.",
                        info.ProviderDisplayName, username);
                else
                    _logger.LogError("Failed to add login '{provider}' for user '{username}': {message}",
                        info.ProviderDisplayName, username, result.Errors.FirstOrDefault());
            }
            else
                _logger.LogError("Failed to create user with username '{username}': {message}.",
                    username, result.Errors.FirstOrDefault());

            return result.Succeeded ? newUser : null;
        }
    }
}