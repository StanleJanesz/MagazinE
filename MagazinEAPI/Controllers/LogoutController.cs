using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("logout")]
    public class LogoutController : ControllerBase
    {
        [HttpPost(Name = "Logout user")]
        [Authorize]
        public async Task<IActionResult> LogoutUser()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            HttpContext.Response.Redirect("/");
            
            return Ok("User logged out successfully");
        }
    }
}
