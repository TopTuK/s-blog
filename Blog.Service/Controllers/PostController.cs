using Blog.Domain.Services.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Service.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class PostController(IPostService postService, ILogger<PostController> logger) : ControllerBase
    {
        private readonly ILogger<PostController> _logger = logger;
        private readonly IPostService _postService = postService;

        public async Task<IActionResult> GetRecentPosts()
        {
            throw new NotImplementedException();
        }
    }
}
