using System.Security.Claims;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTO_Classes;
using MagazinEAPI.Models.Requests;
using SharedLibrary.Base_Classes___Database;

namespace MagazinEAPI.Controllers
{
	[ApiController]
	[Route("unbanRequests")]
	public class UnbanRequestsController : ControllerBase
	{
		private readonly RolesBasedContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public UnbanRequestsController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<IActionResult> Post([FromQuery] UnbanRequestDTO unbanRequesDTO)
		{
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

			var reader = _context.Readers.Include(a => a.Bans).ThenInclude(b => b.UnbanRequests).FirstOrDefault(a => a.ApplicationUserId == userAdminReader.Id);
			if (reader == null)
			{
				return NotFound("You are not in database");
			}
			if (reader.Bans == null)
			{
				return NotFound("Reader Bans null");
			}


			var ban = reader.Bans.Where(b => b.Id == unbanRequesDTO.BanId).FirstOrDefault();
			if (ban == null)
			{
				return NotFound("Ban not found");
			}

			try
			{
				UnbanRequest req = new UnbanRequest()
				{
					BanId = ban.Id,
					Ban = ban,
					Reason = unbanRequesDTO.Reason,
					State = UnbanRequestState.Pending,
					SolvedBy = null,
					SolvedById = null
				};

				_context.Add(req);
				await _context.SaveChangesAsync();

				return Created();

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}


		}

		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Resolve([FromRoute] int id, [FromQuery] UnbanRequestDTO unbanRequesDTO)
		{
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

			var admin = _context.Admins.Include(a => a.Bans).ThenInclude(b => b.UnbanRequests).FirstOrDefault(a => a.ApplicationUserId == userAdminReader.Id);
			if (admin == null)
			{
				return Unauthorized("You are not an admin");
			}
			if (admin.Bans == null)
			{
				return NotFound("Admin Bans null");
			}


			if (unbanRequesDTO.State == UnbanRequestState.Pending)
			{
				return BadRequest("You are not resolving this request correctly");
			}


			var req = _context.UnbanRequests.Include(r => r.Ban).ThenInclude(b => b.User).ThenInclude(u => u.ApplicationUser).Where(r => r.Id == id).FirstOrDefault();
			if (req == null)
			{
				return NotFound("no such unban request");
			}
			if (req.State != UnbanRequestState.Pending)
			{
				return BadRequest("Request already resolved");
			}


			if (req.Ban == null)
			{
				return NotFound("No such ban");
			}
			if (req.Ban.User == null)
			{
				return NotFound("No user to unban");
			}
			if (req.Ban.User.ApplicationUser == null)
			{
				return NotFound("No user to unban");
			}


			if (req.Ban.Active == false) //ban moze byc nieaktywny juz a user zablokowany innym banem, wiec to musi byc najpierw
			{
				//czyli uniewazniamy ten request
				try
				{
					req.State = UnbanRequestState.Denied;
					req.SolvedBy = admin;
					req.SolvedById = admin.Id;

					await _context.SaveChangesAsync();
				}
				catch {}

				return NotFound("ban is not active anymore");
			}
			if (req.Ban.User.ApplicationUser.State != UserState.Banned) //ban jest aktywny a user nie jest banned, error
			{
				return NotFound("User is not banned, data error");
			}


			try
			{
				if (unbanRequesDTO.State == UnbanRequestState.Accepted)
				{
					req.Ban.Active = false;
					req.Ban.BanEndDate = DateTime.UtcNow;
					req.Ban.User.ApplicationUser.State = UserState.Active;
				}
				req.State = unbanRequesDTO.State;
				req.SolvedBy = admin;
				req.SolvedById = admin.Id;
				await _context.SaveChangesAsync();

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest("Could not resolve");
			}
		}

		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin, Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<UnbanRequestDTO>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Get([FromRoute] int id)
		{
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

			var req = _context.UnbanRequests.Include(r => r.Ban).FirstOrDefault(b => b.Id == id);
			if (req == null)
			{
				return NotFound("No such unban request");
			}
			if (req.Ban == null)
			{
				return NotFound("No ban to this unban request");
			}


			//jezeli jestem adminem albo [jestem uzytkownikiem i to moj ban]
			if (!_context.Admins.Any(a => a.ApplicationUserId == userAdminReader.Id) &&
				!_context.Readers.Any(a => a.ApplicationUserId == userAdminReader.Id && a.Id == req.Ban.UserId))
			{
				return Unauthorized("You are not an admin/banned user authorised to have info about this ban");
			}


			return Ok(req.toDTO());
		}

		[HttpGet("user/{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin, Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<List<UnbanRequestDTO>>(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetUserRequests([FromRoute] int id) //po id usera pobieramy jego requesty o unban
		{
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

			var reader = _context.Readers.Include(r => r.Bans).ThenInclude(b => b.UnbanRequests).FirstOrDefault(r => r.Id == id);
			if (reader == null)
			{
				return NotFound("Reader not found");
			}
			if (reader.Bans == null)
			{
				return NotFound("Reader Bans null");
			}

			//jezeli jestem adminem albo [jestem uzytkownikiem i to moj ban]
			if (!_context.Admins.Any(a => a.ApplicationUserId == userAdminReader.Id) &&
				!_context.Readers.Any(r => r.ApplicationUserId == userAdminReader.Id && r.Id == reader.Id))
			{
				return Unauthorized("You are not an admin/banned user authorised to have info about this ban");
			}

			try
			{
				var dtos = reader.Bans.Select(b => b.UnbanRequests).SelectMany(list => list).Select(req => req.toDTO());
				return Ok(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest("Could not return the list");
			}


		}

		[HttpGet("admin")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<List<UnbanRequestDTO>>(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAdminRequests() //u admina pobieramy requesty rozwiazane przez niego
		{
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

			var admin = _context.Admins.Include(a => a.SolvedUnbanRequests).FirstOrDefault(a => a.ApplicationUserId == userAdminReader.Id);
			if (admin == null)
			{
				return Unauthorized("You are not an admin");
			}
			if (admin.SolvedUnbanRequests == null)
			{
				return NotFound("Admin Bans null");
			}


			try
			{
				var dtos = admin.SolvedUnbanRequests.Select(req => req.toDTO());
				return Ok(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest("Could not return the list");
			}
		}


		[HttpGet("pending")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<List<UnbanRequestDTO>>(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetPendingRequests() //u admina pobieramy pending requests
		{
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

			var admin = _context.Admins.FirstOrDefault(a => a.ApplicationUserId == userAdminReader.Id);
			if (admin == null)
			{
				return Unauthorized("You are not an admin");
			}

			var pending = _context.UnbanRequests.Where(r => r.State == UnbanRequestState.Pending);
			if (pending == null)
			{
				return NotFound("No pending unban requests");
			}


			try
			{
				var dtos = pending.Select(req => req.toDTO());
				return Ok(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest("Could not return the list");
			}
		}

	}
}
