namespace MagazinEAPI.Controllers
{
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Requests;
    using MagazinEAPI.Models.Users;
    using MagazinEAPI.Models.Users.Readers;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.Base_Classes___Database;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Controller for handling user registration operations.
    /// </summary>
    [ApiController]
    [Route("register")]
    public class RegistrationController : ControllerBase
    {
        private readonly RolesBasedContext apiContext;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IUserEmailStore<ApplicationUser> emailStore;
        private readonly ILogger<RegistrationController> logger;
        private readonly RoleManager<ApplicationRole> roleManager;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationController"/> class.
        /// </summary>
        /// <param name="apiContext">Databse Context</param>
        /// <param name="userManager">Provides API for managing user in presistence store.</param>
        /// <param name="userStore">Provides API for managing user</param>
        /// <param name="signInManager">Provides API for managing user sign-in's.</param>
        /// <param name="logger">Provides API for managing logs.</param>
        /// <param name="roleManager">Provides API for managing roles in presistence store.</param>
        public RegistrationController(
            RolesBasedContext apiContext,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegistrationController> logger,
            RoleManager<ApplicationRole> roleManager)
        {

            this.apiContext = apiContext;
            this.userManager = userManager;
            this.userStore = userStore;
            emailStore = this.GetEmailStore();
            this.signInManager = signInManager;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        [HttpPost(Name = "Register User")]
        public async Task<IActionResult> Post(RegisterRequestDTO registerRequest)
        {
            string returnUrl = Url.Content("~/");
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            RegisterRequest rr = new()
            {
                IsSuccesfull = false,
                Email = registerRequest.Email,
                LastName = registerRequest.LastName,
                FirstName = registerRequest.FirstName,
                RegisterDateTime = DateTime.Now
            };


            //if (ModelState.IsValid)
            //jezeli nie mamy jeszcze  usera z tym emailem
            if (await userManager.FindByEmailAsync(registerRequest.Email) == null)
            {
                var user = CreateApplicationUser();

                user.FirstName = registerRequest.FirstName;
                user.LastName = registerRequest.LastName;
                user.State = UserState.Active;

                await userStore.SetUserNameAsync(user, registerRequest.Email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, registerRequest.Email, CancellationToken.None);

                var result = await userManager.CreateAsync(user, registerRequest.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");


                    // Jeśli wymagane jest potwierdzenie konta przed logowaniem
                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //trzeba bedzie zminaic w przyszlosci
                        return BadRequest(new { errors = "RequireConfirmedAccount" });
                    }
                    else
                    {
                        //tworzymy usera-czytelnika, jezeli jakis bląd to cofamy rejetrację
                        if (!(await CreateAndAddReader(user)))
                        {
                            await Delete(registerRequest);
                            await apiContext.AddAsync(rr);
                            await apiContext.SaveChangesAsync();

                            return BadRequest(new { errors = "User-Reader with this email already exists, contact our Team" });
                        }

                        rr.IsSuccesfull = true;
                        await apiContext.AddAsync(rr);
                        await apiContext.SaveChangesAsync();

                        //tutaj sie logujemy
                        await signInManager.SignInAsync(user, isPersistent: false);

                        return Ok("User registered successfully");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }


                await apiContext.AddAsync(rr);
                await apiContext.SaveChangesAsync();
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await apiContext.AddAsync(rr);
            await apiContext.SaveChangesAsync();
            return BadRequest(new { errors = "We already have user with this email" });
        }

        //TO MA BYC PRYWATNE
        [HttpDelete(Name = "Delete User")]
        private async Task<IActionResult> Delete(RegisterRequestDTO registerRequest)
        {
            var user = await userManager.FindByEmailAsync(registerRequest.Email);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "User deleted successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }




        private async Task<bool> CreateAndAddReader(ApplicationUser applicationUser)
        {
            if (apiContext.Readers.Include(r => r.ApplicationUser).Any(r => r.ApplicationUser.Email == applicationUser.Email))
                return false;

            User user = new User();
            var appUserID = await userManager.GetUserIdAsync(applicationUser);
            //user.ApplicationUser = applicationUser;

            //applicationUser.User = user;
            await userManager.AddToRoleAsync(applicationUser, "Reader");

            await apiContext.AddAsync(new User { ApplicationUserId = appUserID });
            await apiContext.SaveChangesAsync();

            return true;
        }

        private ApplicationUser CreateApplicationUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }
    }
}
