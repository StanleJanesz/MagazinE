using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
using Xunit;

namespace MagazinEAPI.Controllers
{
	[ApiController]
	[Route("reports")]
	public class ReportController : Controller
	{
		private readonly RolesBasedContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ReportController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Reader, Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType<ArticleDTO>(StatusCodes.Status200OK)]
		public IActionResult Get([FromRoute] int id)
		{
			var report = _context.CommentReports.Include(r => r.Comment).FirstOrDefault(cr => cr.Id == id);
			if (report == null)
			{
				return NotFound("Report not found");
			}

			var comment = report.Comment;
			if(comment == null)
			{
				return NotFound("Comment not found");
			}

			var Email = User.FindFirst(ClaimTypes.Email);
			if (Email == null)
			{
				return BadRequest("Email not found");
			}

			var applicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
			if (applicationUser == null)
			{
				return Unauthorized("App User not found");
			}

			//dowolny admin i ten uzytkownik ktory to zglosił i wlasiciiel zgloszonego komentarza
			if (!_context.Admins.Any(a => a.ApplicationUserId == applicationUser.Id) &&
			   !_context.Readers.Any(r => r.ApplicationUserId == applicationUser.Id && (r.Id == report.ReportAuthorId || r.Id == comment.AuthorId)))
			{
				return Unauthorized("You are not authorised");
			}


			return Ok(report.ToDTO());

		}


		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Reader")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<IActionResult> Post([FromQuery] CommentReportDTO commentReportDTO)
		{
			var Email = User.FindFirst(ClaimTypes.Email);
			if (Email == null)
			{
				return BadRequest("Email not found");
			}

			var applicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
			if (applicationUser == null)
			{
				return Unauthorized("App User not found");
			}

			var reader = _context.Readers.FirstOrDefault(r => r.ApplicationUserId == applicationUser.Id);
			if (reader == null)
			{
				return BadRequest("Reader not found");
			}


			var comment = _context.Comments.FirstOrDefault(c => c.Id == commentReportDTO.CommentId);
			if (comment == null)
			{
				return NotFound("Comment not found");
			}

			//nie sprawdzam czy istnieje poniewaz mozemy wielokrotnie zglosic ten sam komentarz

			try
			{
				var report = new CommentReport()
				{
					ReportAuthorId = reader.Id,
					ReportAuthor = reader,
					CommentId = comment.Id,
					Comment = comment,
					Reason = commentReportDTO.Reason,
					State = CommentReportState.Pending,
					Date = DateTime.UtcNow
				};

				await _context.CommentReports.AddAsync(report);
				await _context.SaveChangesAsync();
			}
			catch
			{
				return BadRequest("Could not create report");
			}


			return Created();

		}



		[HttpPut]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Resolve([FromQuery] CommentReportDTO commentReportDTO)
		{
			var Email = User.FindFirst(ClaimTypes.Email);
			if (Email == null)
			{
				return BadRequest("Email not found");
			}

			var applicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
			if (applicationUser == null)
			{
				return NotFound("App User not found");
			}

			var admin = _context.Admins.FirstOrDefault(a => a.ApplicationUserId == applicationUser.Id);
			if (admin == null)
			{
				return Unauthorized("You are not an andmin");
			}

			var report = _context.CommentReports.Include(cr => cr.Comment).FirstOrDefault(r => r.Id == commentReportDTO.Id);
			if (report == null)
			{
				return NotFound("Report not found");
			}

			var comment = report.Comment;
			if (comment == null)
			{
				return NotFound("Comment not found");
			}

			if (report.State != CommentReportState.Pending)
			{
				return BadRequest("Report already resolved");
			}

			if (commentReportDTO.State == CommentReportState.Pending)
			{
				return BadRequest("Incorrectly resolving report");
			}

			
			try
			{
				if (commentReportDTO.State == CommentReportState.Accepted)
				{
					comment.IsDeleted = true;
					comment.DeletedBy = admin;
					comment.DeletedById = admin.Id;

					//_context.Comments.Update(comment);
				}

				report.State = commentReportDTO.State;
				report.ManagedBy = admin;
				report.ManagedById = admin.Id;

				//_context.CommentReports.Update(report);
				await _context.SaveChangesAsync();
			}
			catch
			{
				return BadRequest("Could not resolve report");
			}

			return Ok(report.ToDTO());
		}


		
	}
}
