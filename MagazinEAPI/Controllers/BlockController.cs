using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MagazinEAPI.Controllers
{
	[ApiController]
	[Route("blocks")]
	public class BlockController
	{
		private readonly RolesBasedContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public BlockController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

	}
}
