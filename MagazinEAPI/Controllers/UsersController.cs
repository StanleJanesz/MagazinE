using Microsoft.AspNetCore.Mvc;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "LoginUser")]
        public IActionResult Get()
        {
            return Ok("User logged in successfully");
        }
    }
}
