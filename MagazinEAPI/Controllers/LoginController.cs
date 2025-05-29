namespace MagazinEAPI.Controllers
{
    using System.Security.Claims;
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models;
    using MagazinEAPI.Models.Users;
    using MagazinEAPI.Models.Users.Readers;
    using MagazinEAPI.utils;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.Base_Classes___Database;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Controller for handling user login operations.
    /// </summary>
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly RolesBasedContext context;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userManager">Provides API for managing user in presistence store.</param>
        public LoginController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// New login operation.
        /// </summary>
        /// <param name="request">login request's info.</param>
        /// <returns>In case of success respose contains JWT token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await this.userManager.FindByEmailAsync(request.Email);
            if (user == null || !await this.userManager.CheckPasswordAsync(user, request.Password))
            {
                return this.Unauthorized("Invalid email or password.");
            }

            var userRoles = await this.userManager.GetRolesAsync(user);
            var token = new JWTCreator().GenerateJwtToken(user, userRoles);

            return this.Ok(new { token });
        }

        /// <summary>
        /// Login with Google.
        /// </summary>
        /// <returns>redirects to Google login.</returns>
        [HttpGet]
        [Route("google")]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/login/google_response",
            };
            return this.Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the response from Google after authentication.
        /// </summary>
        /// <returns>In case of success respose contains JWT token.</returns>
        [HttpGet]
        [Route("google_response")]
        public async Task<ActionResult> GoogleResponse()
        {
            var authentificationResult = await this.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (authentificationResult == null || !authentificationResult.Succeeded)
            {
                return this.BadRequest("Błąd uwierzytelniania");
            }

            var claims = authentificationResult.Principal.Identities.FirstOrDefault().Claims;

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "UNKNOWN";
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "UNKNOWN";

            var appUser = await this.context.Users
                .Include(u => u.Admin)
                .Include(u => u.User)
                .Include(u => u.Journalist)
                .Include(u => u.Editor)
                .Include(u => u.HeadEditor)
                .FirstOrDefaultAsync(u => u.Email == email);

            string role;
            if (appUser == null)
            {
                appUser = await this.CreateUser(name, email);
                role = "User";
            }
            else
            {
                role = new RoleFinder().FindRole(appUser); // TODO: roles in ef
            }

            var dto = new ApplicationUserDTO
            {
                FirstName = name,
                Email = appUser.Email,
                State = appUser.State,
            };

            var jwtCreator = new JWTCreator();
            var jwt = jwtCreator.CreateJWTToken(email, role);

            this.Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2),
                SameSite = SameSiteMode.None,
            });

            return this.Redirect("http://localhost:5173/login");
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
            await this.context.Users.AddAsync(appUser);
            await this.context.SaveChangesAsync();
            return appUser;
        }
    }
}
