using AspNetCore.Authentication.CAS;
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
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(ILogger<AuthController> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
        }

        [HttpGet("umd")]
        public IActionResult UmdLogin([FromQuery] string? returnUrl)
        {
            returnUrl = Url.Action(nameof(UmdLoginCallback), "Auth", new { returnUrl });
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

            IdentityResult result;
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            else if (!signInResult.IsNotAllowed && !signInResult.IsLockedOut)
            {
                var newUser = new User { UserName = info.ProviderKey };
                result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(newUser, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(newUser, isPersistent: true);
                        return LocalRedirect(returnUrl ?? Url.Content("~/"));
                    }
                }
            }
            else
            {
                return BadRequest("User is not allowed to sign in.");
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [Authorize]
        [HttpGet("logout")]
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
            if(name == null)
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
            if (info == null) { return NotFound(); }
            return Ok(info);

        }

    }
}