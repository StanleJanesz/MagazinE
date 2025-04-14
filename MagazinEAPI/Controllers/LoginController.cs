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
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Readers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : ControllerBase
    {
        private readonly RolesBasedContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost("login")]
        [ProducesResponseType(401)]  // Unauthorized (HTTP 401)
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized("Invalid email or password.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = new JWTCreator().GenerateJwtToken(user, userRoles);

            return Ok(new { token });
        }
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
                role = new RoleFinder().FindRole(appUser); // TODO: roles in ef
            }
            var DTO = new ApplicationUserDTO
            {
                FirstName = name,
                Email = appUser.Email,
                State = appUser.State,
            };

            var JWTCreator = new JWTCreator();
            var JWT = JWTCreator.CreateJWTToken(email, role);

            Response.Cookies.Append("JWT",JWT, new CookieOptions 
            { 
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2),
            });

            return Ok("Zalogowano");
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
