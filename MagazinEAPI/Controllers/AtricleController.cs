using MagazinEAPI.Contexts;
using MagazinEAPI.Models;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTO_Classes;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("articles")]
    public class AtricleController : Controller
    {
        private readonly RolesBasedContext _context;

        public AtricleController(RolesBasedContext context)
        {
            _context = context;
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet(Name = "GetArticle")]
        [Authorize]
        public ActionResult<ArticleDTO> Get([FromQuery] int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound("Article not found");
            }

            var articleDTO = article.ToDTO();

            return Ok(articleDTO);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Journalist")]
        [HttpPost(Name = "CreateArticle")]
        public async Task<IActionResult> Post(string Title, string authorEmail)
        {
            if (Title == null || authorEmail == null)
            {
                return BadRequest("Title and author are required");
            }

            var author = await _context.Users.FirstOrDefaultAsync(u => u.Email == authorEmail);
            if (author == null)
            {
                return BadRequest("Author does not exist");
            }
            RoleFinder roleFinder = new RoleFinder();
            var role = roleFinder.FindRole(author);
            if (role != RoleFinder.Journalist)
            {
                return BadRequest("Author is not journalist");
            }

            await _context.Articles.FirstOrDefaultAsync(a => a.Title == Title);
            // TODO: sprawdzanie czy tytuł jest unikalny
            if (false)
            {
                return BadRequest("Title must be unique");
            }

            try
            {
                var result = await _context.Articles.AddAsync(new()
                {
                    Title = Title,
                    Author = author.Journalist,
                });
            }
            catch
            {
                return BadRequest("Adding article failed");
            }

            return Ok("Article created successfully");
        }
    }
}
