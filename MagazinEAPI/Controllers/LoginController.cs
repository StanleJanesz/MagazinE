using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MagazinEAPI.Contexts;

using Microsoft.EntityFrameworkCore;
using MagazinEAPI.Models;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : ControllerBase
    {
        private readonly APIContext _context;

        [HttpPost(Name = "Login User")]
        public async Task<ActionResult<ApplicationUser>> Post()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/",
            };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
            var claims = User.Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
           
            if (user == null) //pierwsze logowanie
            {
                await _context.Users.AddAsync(new Models.ApplicationUser
                {
                    UserName = name,
                    Email = email,
                });
                 user = new ApplicationUser
                {
                    UserName = name,
                    Email = email,
                };
            }
            return user;
        }
    }
}
