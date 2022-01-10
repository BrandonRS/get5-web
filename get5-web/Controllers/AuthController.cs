using AspNetCore.Authentication.CAS;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace get5_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private SignInManager<MongoUser> _signInManager;

        public AuthController(SignInManager<MongoUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("umd")]
        public IActionResult AuthUMD()
        {
            var authProperties = _signInManager
                .ConfigureExternalAuthenticationProperties(CasDefaults.AuthenticationScheme.SCHEME,
                "/api/auth/umd");

            return Challenge(authProperties, CasDefaults.AuthenticationScheme.SCHEME);
        }
    }
}
