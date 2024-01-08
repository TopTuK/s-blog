using Blog.Domain.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController(IConfiguration configuration, IUserService userService,
        ILogger<AuthController> logger) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserService _userService = userService;

        [AllowAnonymous]
        public async Task<IActionResult> SinginCallback()
        {
            _logger.LogInformation("AuthController::SinginCallback: Reading the outcome of external auth");

            // Read the outcome of external auth
            var authResult = await HttpContext.AuthenticateAsync(_configuration["Auth:TempCookieName"]);

            if (!authResult.Succeeded)
            {
                _logger.LogError("AuthController::SinginCallback: Can't read the outcome of external authentication");
                return LocalRedirect(new PathString("/"));
            }

            _logger.LogInformation("AuthController::SinginCallback: Authentication succeeded.");
            // Read metadata with scheme
            var metadata = authResult.Properties.Items;
            if ((metadata == null)
                || (!metadata.ContainsKey("scheme"))
                || (string.IsNullOrEmpty(metadata["scheme"])))
            {
                _logger.LogError("AuthController::SinginCallback: Metadata doesn't contain scheme");
                return LocalRedirect(new PathString("/"));
            }

            var schemeName = metadata["scheme"]!;
            _logger.LogInformation("AuthController::SinginCallback: Authentication scheme name is {}",
                schemeName);

            try
            {
                var user = await _userService.AuthenticateAsync(
                    schemeName: schemeName,
                    claims: authResult.Principal.Claims,
                    metadata: metadata!
                );

                _logger.LogInformation("AuthController::SinginCallback: Authenticated user: {} {} {}",
                    user.Email, user.FirstName, user.LastName);
                
                var claims = new List<Claim>
                {
                    new("sub", user.Id.ToString()),
                    new("email", user.Email),
                };

                if (user.IsAdministrator)
                {
                    claims.Add(new("isAdmin", "yes"));
                }

                // Run user authentification login (First time seen?)
                // var ci = new ClaimsIdentity(claims, "pwd", "name", "role");
                var ci = new ClaimsIdentity(claims, schemeName);
                var cp = new ClaimsPrincipal(ci);

                await HttpContext.SignInAsync(cp);
                await HttpContext.SignOutAsync(_configuration["Auth:TempCookieName"]);

                _logger.LogInformation("AuthController::SinginCallback: Success SignIn user");
                return LocalRedirect(new PathString("/"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "AuthController::SinginCallback: Can't authentificate user");
                return LocalRedirect(new PathString("/"));
            }
        }

        [HttpGet]
        public IActionResult SignInVas3k()
        {
            _logger.LogInformation("AuthController::SignInVas3k: Start Vas3k authentication");

            var schemeName = _configuration["Vas3kAuth:Scheme"];
            if (schemeName == null)
            {
                _logger.LogError("AuthController::SignInVas3k: scheme name is null");
                return BadRequest("Scheme name is null");
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = new PathString("/auth/SinginCallback"),
                Items =
                {
                    { "scheme", schemeName }
                }
            };

            return Challenge(props, schemeName);
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("AuthController::Logout: start routin");

            // Get User Id
            int userId;
            if (!int.TryParse(User.FindFirstValue("sub"), out userId))
            {
                _logger.LogError("AuthController::Logout: User is not authenticated. Can't find user id");
                return BadRequest("Not authenticated");
            }

            //var user = await _userService.GetUserByIdAsync(userId);
            //_logger.LogInformation($"AuthController::Logout: {user?.Email}");

            await HttpContext.SignOutAsync();

            return LocalRedirect(new PathString("/"));
        }
    }
}
