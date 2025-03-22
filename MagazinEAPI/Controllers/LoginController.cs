using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MagazinEAPI.Contexts;

using Microsoft.EntityFrameworkCore;
using MagazinEAPI.Models;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Base_Classes___Database;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : ControllerBase
    {
        private readonly APIContext _context;

        [HttpPost(Name = "Login User")]
        public async Task<ActionResult<PersonInfo>> Post()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/",
                Items = 
                { 
                    { "scheme", GoogleDefaults.AuthenticationScheme },
                    { "login_hint", User.FindFirst(ClaimTypes.Email)?.Value } //nie wiem czy to zadziała, jak coś to jest tylko dodatkowy feature
                }
            };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
            var claims = User.Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var id = Guid.NewGuid().ToString();
            if (appUser == null) //pierwsze logowanie
            {
                appUser = new ApplicationUser
                {
                    UserName = name,
                    Email = email,
                    State = UserState.Active,
                    Id = id,
                };
                await _context.Users.AddAsync(appUser
                );
            }
            var user = new User
            {
                ApplicationUserId = appUser.Id,
                ApplicationUser = appUser,
            };
            appUser.User = user;
            await _context.SaveChangesAsync();
            var DTO = new PersonInfo
            {
                FirstName = name,
                Email = appUser.Email,
                State = appUser.State,
            };
            return DTO;
        }
    }
}
