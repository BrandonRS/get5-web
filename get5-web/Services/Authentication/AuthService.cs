using get5_web.Interfaces.Authentication;
using get5_web.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System.Security.Claims;

namespace get5_web.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private const string SteamProviderDisplayName = "Steam";
        private const string UmdProviderDisplayName = "CAS";

        private ILogger<AuthService> _logger;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private SteamUser _steamUserInterface;

        public AuthService(ILogger<AuthService> logger, SignInManager<User> signInManager, SteamWebInterfaceFactory webInterfaceFactory)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _steamUserInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>();
        }

        #region properties

        private ClaimsPrincipal User
        {
            get => _signInManager.Context.User;
        }

        #endregion properties

        #region public

        public async Task<User?> CreateUserWithLogin(string username, ExternalLoginInfo info)
        {
            _logger.LogInformation("Creating user with username '{username}' for external login '{provider}'.",
                username, info.ProviderDisplayName);

            var newUser = new User { UserName = username };
            var result = false;
            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded)
            {
                _logger.LogInformation("Successfully created user '{username}'.", username);
                result = await AddLoginForUser(newUser, info);
            }
            else
                _logger.LogError("Failed to create user with username '{username}': {message}.",
                    username, identityResult.Errors.FirstOrDefault());

            return result ? newUser : null;
        }

        public async Task<bool> AddLoginForUser(User user, ExternalLoginInfo info)
        {
            var result = await _userManager.AddLoginAsync(user, info);

            if (result.Succeeded)
                _logger.LogInformation("Successfully added login '{provider}' for user '{username}'.",
                    info.ProviderDisplayName, user.UserName);
            else
                _logger.LogError("Failed to add login '{provider}' for user '{username}': {message}",
                    info.ProviderDisplayName, user.UserName, result.Errors.FirstOrDefault());

            return result.Succeeded;
        }

        public bool IsSignedIn() => _signInManager.IsSignedIn(User);

        public async Task<User> GetUserAsync() => await _userManager.GetUserAsync(User);

        public async Task<bool> HasSteamLogin()
        {
            var externalLogins = await _userManager.GetLoginsAsync(await GetUserAsync());

            return externalLogins.Any(login => login.ProviderDisplayName == SteamProviderDisplayName);
        }

        public async Task<bool> HasUmdLogin()
        {
            var externalLogins = await _userManager.GetLoginsAsync(await GetUserAsync());

            return externalLogins.Any(login => login.ProviderDisplayName == UmdProviderDisplayName);
        }

        public async Task<bool> AddUmdLoginForUser(User user, ExternalLoginInfo info)
        {
            var result = await AddLoginForUser(user, info);

            if (result)
            {
                user.UserName = info.ProviderKey;
                await _userManager.UpdateAsync(user);
            }

            return result;
        }

        public async Task<User?> CreateSteamUserWithLogin(ExternalLoginInfo info)
        {
            var steamId = GetUlongForSteamProviderKey(info.ProviderKey);
            if (steamId == null)
                return null;

            var playerSummaryResponse = await _steamUserInterface.GetPlayerSummaryAsync(steamId.Value);
            if (playerSummaryResponse == null)
            {
                _logger.LogError("Failed to get Steam info for SteamID '{steamId}'.", info.ProviderKey);
                return null;
            }

            return await CreateUserWithLogin(playerSummaryResponse.Data.Nickname, info);
        }

        #endregion public

        #region private

        private ulong? GetUlongForSteamProviderKey(string providerKey)
        {
            var steamIdString = providerKey.Split("/").LastOrDefault();
            if (steamIdString == null)
            {
                _logger.LogError("Failed to get SteamID from Steam provider key '{providerKey}'.", providerKey);
                return null;
            }

            if (!ulong.TryParse(steamIdString, out var steamId))
            {
                _logger.LogError("Failed to parse SteamID '{steamId}' to ulong.", steamIdString);
                return null;
            }

            return steamId;
        }

        #endregion private
    }
}