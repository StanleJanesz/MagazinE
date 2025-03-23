using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MagazinEAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using MagazinEAPI.Models;
using MagazinEAPI.utils;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : ControllerBase
    {
        private readonly APIContext _context;


        [HttpPost]
        [Route("google")]

        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "google_response",
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        [Route("google_response")]

        public async Task<ActionResult> GoogleResponse()
        {
            var authentificationResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authentificationResult.Succeeded)
            {
                return BadRequest("Błąd uwierzytelniania");
            }

            var claims = authentificationResult.Principal.Identities.FirstOrDefault().Claims;

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            string role;
            if (appUser == null)
            {
                appUser = await CreateUser(name, email);
                role = "User";
            }
            else
            {
                role = new RoleFinder().FindRole(appUser);
            }
            var DTO = new ApplicationUserDTO
            {
                FirstName = name,
                Email = appUser.Email,
                State = appUser.State,
            };

            var JWT = CreateJWTToken(email,role);

            Response.Cookies.Append("JWT",JWT, new CookieOptions 
            { 
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2),
            });

            return Ok("Zalogowano");
        }

        private string CreateJWTToken(string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key")); // TODO: move to secrets and change to real one
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

            var token = new JwtSecurityToken(
                issuer: "MagazinEServer",
                audience: "MagazinEClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> CreateUser(string name, string email)
        {
            var appUser = new ApplicationUser
            {
                UserName = name,
                Email = email,
                State = UserState.Active,
            };
            var user = new User
            {
                ApplicationUserId = appUser.Id,
                ApplicationUser = appUser,
            };
            appUser.User = user;
            await _context.Users.AddAsync(appUser);
            await _context.SaveChangesAsync();
            return appUser;
        }
    }
}
