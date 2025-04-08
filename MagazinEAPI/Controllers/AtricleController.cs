using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users;
using MagazinEAPI.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTO_Classes;
using System.Security.Claims;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("articles")]
    public class AtricleController : Controller
    {
        private readonly RolesBasedContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AtricleController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
       
        [HttpGet("{id}")]
        [Authorize]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        [ProducesResponseType<ArticleDTO>(StatusCodes.Status200OK)]
        public IActionResult Get([FromRoute] int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound("Article not found");
            }

            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return BadRequest("User not found");
            }

            if (!article.CanBeViewedBy(ApplicationUser, _context, _userManager))
            {
                return Unauthorized("User cannot view this article");
            }

            return Ok(article.ToDTO());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Journalist, Editor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Put([FromRoute] int id, [FromBody] ArticleDTO articleDTO)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound("Article not found");
            }

            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return BadRequest("User not found");
            }

            if (!article.CanBeEdited(ApplicationUser, _context, _userManager))
            {
                return Unauthorized("User cannot edit this article");
            }

            if (articleDTO == null)
            {
                return BadRequest("Request is empty");
            }

            try
            {
                article.Title = articleDTO.Title;
                article.Content = articleDTO.Content;
                article.Introduction = articleDTO.Introduction;
                // TODO: deciede how we want to handle photos 
                // TODO: tags should be in another call (editor can not change tags?  only author and headEditor ?)
                // TODO: publish and premium should be in another call (only Editor HeadEditor but not Author)
                _context.Articles.Update(article);
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest("Editing article failed");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Journalist, Editor, HeadEditor")]

        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Delete([FromRoute] int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound("Article not found");
            }

            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return BadRequest("User not found");
            }

            if (!article.CanBeDelatedBy(ApplicationUser, _context, _userManager))
            {
                return Unauthorized("User cannot delate this article");
            }

            try
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest("Deleting article failed");
            }

            return Ok();
        }   

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ICollection<ArticleDTO>>(StatusCodes.Status200OK)]
        // Not authorized becouse it is designed to get articles for main page
        public IActionResult Get([FromQuery]ArticlesRequestDTO articlesRequestDTO) // TODO: decide what kind of filter will be used and which orders will be available
        {
            if (articlesRequestDTO == null)
            {
                return BadRequest("Request is empty");
            }
            int skip = articlesRequestDTO.BatchSize * articlesRequestDTO.Page;

            List<Article> articles;

            try // Ugly code, to be refactored
            {
                articles = _context.Articles
                    .Where( a => a.isPublished )
                   // .Where( a => articlesRequestDTO.Tags.All(t => a.Tags.Any(tag => tag.Id == t))) // dont know what kinds of filters will be used
                   // .Where(a => articlesRequestDTO.Authors.Any(author => a.Author.Id == author))  // dont know what kinds of filters will be used
                   .OrderBy(a => a.TimeOfPublication) // TODO: decide which order will be default
                   .Skip(skip)
                   .Take(articlesRequestDTO.BatchSize)
                   .ToList();
            }
            catch
            {
                return BadRequest("Getting articles failed");
            }

            return Ok(articles.Select(a => a.ToInfoDTO()));
        }



        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Journalist")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromQuery]ArticleDTO articleDTO) // content can be empty will be added later,Title is oblligatory,  email of author is taken from token 
                                                                                // also could consider adding from only string title as other can be included later using put
                                                                                // i would also consider making content and introduction optional
        {
            var email = User.FindFirst(ClaimTypes.Email);
            if (articleDTO == null || articleDTO.Title == null)
            {
                return BadRequest("Title is required");
            }

            var user = await _userManager.FindByEmailAsync(email.Value);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var journalist = _context.Journalists.FirstOrDefault(j => j.ApplicationUserId == user.Id);
            if (journalist == null)
            {
                return Unauthorized("User is not a journalist");
            }

            if (_context.Articles.Any(a => a.Title == articleDTO.Title)) // TODO: we have to decide if we want to allow articles with the same title
            {
                return BadRequest("Article with this title already exists");
            }

            try
            {
                _context.Articles.Add(new Article
                {
                    Title = articleDTO.Title,
                    Content = articleDTO.Content,
                    Introduction = articleDTO.Introduction,
                    isPremium = false,
                    isPublished = false,
                    AuthorId = journalist.Id,
                    TimeOfPublication = null,
                    Author = journalist,
                });
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest("Adding article failed");
            }

            return Created();
        }
    }
}
