using Microsoft.AspNetCore.Mvc;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("register")]
    public class RegistrationController : ControllerBase
    {
        
        [HttpPost(Name = "Register User")]
        public IActionResult Post()
        {
            return Ok("User registered successfully");
        }
    }
}
