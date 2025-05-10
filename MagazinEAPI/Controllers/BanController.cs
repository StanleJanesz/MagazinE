using System.Security.Claims;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTO_Classes;
using SharedLibrary.Base_Classes___Database;
using Microsoft.EntityFrameworkCore;
using MagazinEAPI.Migrations;

namespace MagazinEAPI.Controllers
{
	[ApiController]
	[Route("ban")]
	public class BanController : ControllerBase
	{
		private readonly RolesBasedContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public BanController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<IActionResult> Post([FromQuery] BanDTO banDTO)
		{
			var email = User.FindFirst(ClaimTypes.Email);
			if (email == null)
			{
				return BadRequest("Your email not found");
			}

			if (banDTO == null)
			{
				return BadRequest("Null request");
			}

			var userAdmin = await _userManager.FindByEmailAsync(email.Value);
			if (userAdmin == null)
			{
				return BadRequest("App user not found");
			}

			var admin = _context.Admins.FirstOrDefault(a => a.ApplicationUserId == userAdmin.Id);
			if (admin == null)
			{
				return Unauthorized("You are not an admin");
			}

			var reader = _context.Readers.Include(r => r.ApplicationUser).FirstOrDefault(r => r.Id == banDTO.UserId);
			if (reader == null || reader.ApplicationUser == null)
			{
				return BadRequest("No such user to ban");
			}

			var userReader = reader.ApplicationUser;

			if (userReader.State == UserState.Banned)
			{
				return BadRequest("User is already Banned");
			}

			if (_context.Bans.Any(b => b.UserId == reader.Id && b.Active))
			{
				return BadRequest("User is already Banned");
			}


			try
			{
				_context.Bans.Add(new Ban()
				{
					UserId = reader.Id,
					AdminId = admin.Id,
					User = reader,
					Admin = admin,
					Reason = banDTO.Reason,
					Active = true,
					BanStartDate = DateTime.UtcNow,
					BanEndDate = null
				});

				userReader.State = UserState.Banned;
				_context.Users.Update(userReader);


				await _context.SaveChangesAsync();
			}
			catch
			{
				return BadRequest("Cannot ban user");
			}


			return Created();
		}

		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Put([FromRoute] int id, [FromBody] BanDTO banDTO) //Unban for example, if in body we have Active == false
		{
			var email = User.FindFirst(ClaimTypes.Email);
			if (email == null)
			{
				return BadRequest("Your email not found");
			}

			if (banDTO == null)
			{
				return BadRequest("Null request");
			}

			var ban = _context.Bans.FirstOrDefault(b => b.Id == id);
			if (ban == null)
			{
				return NotFound("No such ban");
			}

			var userAdmin = await _userManager.FindByEmailAsync(email.Value);
			if (userAdmin == null)
			{
				return BadRequest("App user not found");
			}

			//zakladam ze tylko "wlasiciel" bana moze cofnac ban w tym endpoint
			if (!_context.Admins.Any(a => a.ApplicationUserId == userAdmin.Id && a.Id == ban.AdminId))
			{
				return Unauthorized("You are not an admin authorised to unban user");
			}


			var reader = _context.Readers.Include(r => r.ApplicationUser).FirstOrDefault(r => r.Id == banDTO.UserId);
			if (reader == null || reader.ApplicationUser == null)
			{
				return NotFound("No such user to unban");
			}

			var userReader = reader.ApplicationUser;


			try
			{
				if (!banDTO.Active)
				{
					ban.Active = banDTO.Active;
					ban.BanEndDate = banDTO.BanEndDate ?? DateTime.UtcNow;

					userReader.State = UserState.Active;
					_context.Users.Update(userReader);
					_context.Bans.Update(ban);

					await _context.SaveChangesAsync();
				}
			}
			catch
			{
				return BadRequest("Could not unban");
			}

			return Ok();
		}


		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin, Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<BanDTO>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Get([FromRoute] int id)
		{
			var ban = _context.Bans.Include(b => b.UnbanRequests).FirstOrDefault(b => b.Id == id);
			if (ban == null)
			{
				return NotFound("No such ban");
			}

			var email = User.FindFirst(ClaimTypes.Email);
			if (email == null)
			{
				return BadRequest("Email not found");
			}

			//admin banujący i uzytkownik zbanowany moga pobrac bana:
			var userAdminReader = await _userManager.FindByEmailAsync(email.Value);
			if (userAdminReader == null)
			{
				return BadRequest("App user not found");
			}

			//jezeli jestem adminem albo [jestem uzytkownikiem i to moj ban]
			if (!_context.Admins.Any(a => a.ApplicationUserId == userAdminReader.Id) &&
				!_context.Readers.Any(a => a.ApplicationUserId == userAdminReader.Id && a.Id == ban.UserId))
			{
				return Unauthorized("You are not an admin/banned user authorised to have info about this ban");
			}


			return Ok(ban.toDTO());
		}

		[HttpGet]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin, Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<List<BanDTO>>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Get([FromBody] BansRequestDTO bansRequestDTO)
		{
			if (bansRequestDTO == null)
			{
				return BadRequest("Null request");
			}

			if (bansRequestDTO.BanIds.Count == 0)
			{
				return BadRequest("No ids");
			}


			var email = User.FindFirst(ClaimTypes.Email);
			if (email == null)
			{
				return BadRequest("Email not found");
			}

			var userAdminReader = await _userManager.FindByEmailAsync(email.Value);
			if (userAdminReader == null)
			{
				return BadRequest("App user not found");
			}


			List<Ban> bans = [];
			foreach (int id in bansRequestDTO.BanIds)
			{
				Ban? ban = await _context.Bans.Include(b => b.UnbanRequests).FirstOrDefaultAsync(b => b.Id == id);

				if (ban == null)
				{
					return NotFound("No such ban");
				}

				bans.Add(ban);
			}

			//admin pobiera info o jakichkolwiek banach
			if (_context.Admins.Any(a => a.ApplicationUserId == userAdminReader.Id))
			{
				return Ok(bans.Select(b => b.toDTO()));
			}


			//usytkonwik moze pobrac tylko swoje bany
			var usersIds = bans.Select(b => b.UserId).Distinct();
			if (usersIds.Count() != 1)
			{
				return Unauthorized("Bans have different different users");
			}

			var userId = usersIds.FirstOrDefault();
			if (!_context.Readers.Any(r => r.ApplicationUserId == userAdminReader.Id && r.Id == userId))
			{
				return Unauthorized("You are not an banned user authorised to have info about this bans");
			}

			return Ok(bans.Select(b => b.toDTO()));
		}


		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin, Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<List<int>>(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetUserBansId([FromRoute] int id) //reader id - id swoich banow lub admin pobiera bany danego readera
		{
			var email = User.FindFirst(ClaimTypes.Email);
			if (email == null)
			{
				return BadRequest("Email not found");
			}

			//admin banujący i uzytkownik zbanowany moga pobrac bana:
			var applicationUser = await _userManager.FindByEmailAsync(email.Value);
			if (applicationUser == null)
			{
				return BadRequest("App user not found");
			}

			var reader = _context.Readers.Include(r => r.Bans).FirstOrDefault(r => r.Id == id);
			if(reader == null)
			{
				return NotFound("Reader not found");
			}
			if (reader.Bans == null)
			{
				return NotFound("Reader Bans not found");
			}


			//jezeli jestem adminem albo jestem poszukiwanym uzytkownikiem 
			if (!_context.Admins.Any(a => a.ApplicationUserId == applicationUser.Id) &&
				!_context.Readers.Any(r => r.ApplicationUserId == applicationUser.Id && r.Id == reader.Id))
			{
				return Unauthorized("You are not an admin/banned user authorised to have info about this ban");
			}


			return Ok(reader.Bans.Select(b => b.Id));
		}

		[HttpGet]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<List<int>>(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdminBansId() //admin pobiera swoje bany
		{
			var email = User.FindFirst(ClaimTypes.Email);
			if (email == null)
			{
				return BadRequest("Email not found");
			}

			var applicationUser = await _userManager.FindByEmailAsync(email.Value);
			if (applicationUser == null)
			{
				return BadRequest("App user not found");
			}

			var admin = _context.Admins.Include(a => a.Bans).FirstOrDefault(a => a.ApplicationUserId == applicationUser.Id);
			if (admin == null)
			{
				return Unauthorized("You are not an admin");
			}
			if (admin.Bans == null)
			{
				return NotFound("Not found bans");
			}


			return Ok(admin.Bans.Select(b => b.Id));
		}

	}
}
