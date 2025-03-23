using Microsoft.AspNetCore.Mvc;

using MagazinEAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using SharedLibrary.DTO_Classes;
using SharedLibrary.Base_Classes___Database;
using MagazinEAPI.Contexts;
using Microsoft.EntityFrameworkCore;
namespace MagazinEAPI.Controllers
{
	[ApiController]
	[Route("register")]
	public class RegistrationController : ControllerBase
	{
		private readonly APIContext _APIContext;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;
		private readonly ILogger<RegistrationController> _logger;

		public RegistrationController(APIContext APIContext,
			UserManager<ApplicationUser> userManager,
			IUserStore<ApplicationUser> userStore,
			SignInManager<ApplicationUser> signInManager,
			ILogger<RegistrationController> logger)
		{
			_APIContext = APIContext;
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
		}


		//public string ReturnUrl { get; set; }
		public IList<AuthenticationScheme> ExternalLogins { get; set; }


		[HttpPost(Name = "Register User")]
		public async Task<IActionResult> Post(RegisterRequestDTO registerRequest)
		{
			string returnUrl = Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
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
			if (await _userManager.FindByEmailAsync(registerRequest.Email) == null)
			{
				var user = CreateApplicationUser();

				user.FirstName = registerRequest.FirstName;
				user.LastName = registerRequest.LastName;
				user.State = UserState.Active;

				await _userStore.SetUserNameAsync(user, registerRequest.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, registerRequest.Email, CancellationToken.None);

				var result = await _userManager.CreateAsync(user, registerRequest.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");


					// Jeśli wymagane jest potwierdzenie konta przed logowaniem
					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						//trzeba bedzie zminaic w przyszlosci
						return BadRequest(new { errors = "RequireConfirmedAccount" });
					}
					else
					{
						//tworzymy usera-czytelnika, jezeli jakis bląd to cofamy rejetrację
						if(!(await CreateAndAddReader(user)))
						{
							await Delete(registerRequest);
							await _APIContext.AddAsync(rr);
							await _APIContext.SaveChangesAsync();

							return BadRequest(new { errors = "User-Reader with this email already exists, contact our Team" });
						}

						rr.IsSuccesfull = true;
						await _APIContext.AddAsync(rr);
						await _APIContext.SaveChangesAsync();

						//tutaj sie logujemy
						await _signInManager.SignInAsync(user, isPersistent: false);

						return Ok("User registered successfully");
					}
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}


				await _APIContext.AddAsync(rr);
				await _APIContext.SaveChangesAsync();
				return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
			}

			await _APIContext.AddAsync(rr);
			await _APIContext.SaveChangesAsync();
			return BadRequest(new { errors = "We already have user with this email" });
		}


		//TO MA BYC PRYWATNE
		[HttpDelete(Name = "Delete User")]
		private async Task<IActionResult> Delete(RegisterRequestDTO registerRequest)
		{
			var user = await _userManager.FindByEmailAsync(registerRequest.Email);

			if (user == null)
			{
				return NotFound(new { message = "User not found" });
			}

			var result = await _userManager.DeleteAsync(user);

			if (result.Succeeded)
			{
				return Ok(new { message = "User deleted successfully" });
			}

			return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
		}




		private async Task<bool> CreateAndAddReader(ApplicationUser applicationUser)
		{
			if (_APIContext.Readers.Include(r => r.ApplicationUser).Any(r => r.ApplicationUser.Email == applicationUser.Email))
				return false;

			User user = new User();
			user.ApplicationUserId = await _userManager.GetUserIdAsync(applicationUser);
			user.ApplicationUser = applicationUser;

			applicationUser.User = user;

			await _APIContext.AddAsync(user);
			await _APIContext.SaveChangesAsync();

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
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<ApplicationUser>)_userStore;
		}
	}
}
