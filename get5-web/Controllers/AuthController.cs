using AspNetCore.Authentication.CAS;
using get5_web.Interfaces.Authentication;
using get5_web.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace get5_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string SteamScheme = "Steam";

        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;

        #region public

        public AuthController(ILogger<AuthController> logger, SignInManager<User> signInManager, IAuthService authService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _authService = authService;
        }

        [HttpGet("steam")]
        public IActionResult SteamLogin([FromQuery] string? returnUrl)
        {
            returnUrl = Url.Action(nameof(SteamLoginCallback), "auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(SteamScheme, returnUrl);
            return Challenge(properties, SteamScheme);
        }

        [Authorize(AuthenticationSchemes = SteamScheme)]
        [HttpGet("steam/callback")]
        public async Task<IActionResult> SteamLoginCallback([FromQuery] string? returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(SteamLogin));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            else if (!signInResult.IsNotAllowed && !signInResult.IsLockedOut)
            {
                return LocalRedirect("~/profile/newuser");
            }
            else
            {
                return BadRequest("User is not allowed to sign in.");
            }
        }

        [HttpGet("umd")]
        public IActionResult UmdLogin([FromQuery] string? returnUrl)
        {
            returnUrl = Url.Action(nameof(UmdLoginCallback), "auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                CasDefaults.AuthenticationScheme.SCHEME, returnUrl);
            return Challenge(properties, CasDefaults.AuthenticationScheme.SCHEME);
        }

        [Authorize(AuthenticationSchemes = CasDefaults.AuthenticationScheme.SCHEME)]
        [HttpGet("umd/callback")]
        public async Task<IActionResult> UmdLoginCallback([FromQuery] string? returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(UmdLogin));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User '{username}' successfully signed in.", info.ProviderKey);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            else if (!signInResult.IsNotAllowed && !signInResult.IsLockedOut)
            {
                var newUser = await _authService.CreateUserWithLogin(info.ProviderKey, info);

                if (newUser != null)
                {
                    _logger.LogInformation("User '{username}' successfully signed in.", newUser.UserName);
                    await _signInManager.SignInAsync(newUser, isPersistent: true);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
                else
                    return LocalRedirect("~/");
            }
            else
            {
                _logger.LogError("Failed to log in user '{username}': User is not allowed to sign in ({message}).",
                    info.ProviderKey, signInResult.ToString());
                return LocalRedirect("~/");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("~/");
        }

        [Authorize]
        [HttpGet("username")]
        public IActionResult GetUserName()
        {
            var name = _signInManager.Context.User?.Identity?.Name;
            if (name == null)
            {
                return NotFound();
            }
            return Ok(name);
        }

        [Authorize]
        [HttpGet("userInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var info = await _userManager.GetUserAsync(User);
            if (info == null)
            {
                return NotFound();
            }
            return Ok(info);
        }

        #endregion public
    }
}