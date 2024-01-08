using Blog.Domain.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Service.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        public async Task<IActionResult> GetUser()
        {
            // Get User Id (not null)
            var userId = (int)HttpContext.Items["userId"]!;
            _logger.LogInformation("UserController::GetUser: Id={}", userId);

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("UserController::GetUser: user with Id={} is not found", userId);
                return NotFound("User is not found");
            }

            _logger.LogInformation("UserController::GetUser: found user {} {} {}",
                user.Email, user.FirstName, user.LastName);
            return new JsonResult(user);
        }
    }
}
