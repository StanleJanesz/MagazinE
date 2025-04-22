using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.utils;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTO_Classes;
using System.Security.Claims;
using MagazinEAPI.Models.Articles.Comment;

namespace MagazinEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {

        private readonly RolesBasedContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
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
        [ProducesResponseType<CommentDTO>(StatusCodes.Status200OK)]
        public IActionResult Get(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            if (!comment.Article.isPublished)
            {
                return Unauthorized("Comment cannot be viewed because article is not published");
            }

            if (comment.IsDeleted && !User.IsInRole("Admin"))
            {
                return Unauthorized("Comment cannot be viewed because it is deleted");
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

            if (!comment.Article.CanBeViewedBy(ApplicationUser, _context, _userManager))
            {
                return Unauthorized("User cannot view this comment");
            }

            return Ok(comment.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CommentDTO commentDTO)
        {
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

            var article = _context.Articles.FirstOrDefault(a => a.Id == commentDTO.ArticleId);
            if (article == null)
            {
                return NotFound("Article not found");
            }

            if (!article.CanBeViewedBy(ApplicationUser, _context, _userManager))
            {
                return Unauthorized("User cannot view this article");
            }

            var comment = new Comment();
            comment.UpdateFromDTO(commentDTO);

            if (commentDTO.ParentId != null)
            {
                var parent = _context.Comments.FirstOrDefault(c => c.Id == commentDTO.ParentId);
                if (parent == null)
                {
                    return NotFound("Parent comment not found");
                }
                comment.Parent = parent;
            }
            comment.Author = ApplicationUser.User;
            comment.Article = article;
            comment.AuthorId = ApplicationUser.User.Id;
            _context.Add(comment);
            _context.SaveChanges();


            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Ok(comment.ToDTO());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Delete(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
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


            comment.IsDeleted = true;
            comment.DeletedBy = ApplicationUser.Admin;
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}/restore")]
        [Authorize(Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Restore(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            comment.IsDeleted = false;
            comment.DeletedBy = null;
            _context.SaveChanges();

            return Ok();
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Put(int id, [FromBody] CommentDTO commentDTO)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
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

            if (comment.AuthorId != ApplicationUser.User.Id)
            {
                return Unauthorized("User can only edit his comments");
            }

            comment.Content = commentDTO.Content;

            _context.SaveChanges();

            return Ok(comment.ToDTO());
        }

        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        [HttpPost("{id}/likes")]
        public IActionResult Like([FromRoute] int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
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

            if (comment.LikeUsers.Contains(ApplicationUser.User))
            {
                return BadRequest("User already liked this comment");
            }

            comment.LikeUsers.Add(ApplicationUser.User);
            _context.SaveChanges();

            return Ok(comment.LikeUsers.Count);
        }

        [HttpDelete("{id}/likes")]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public IActionResult DelateLike([FromRoute] int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
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

            if (!comment.LikeUsers.Contains(ApplicationUser.User))
            {
                return BadRequest("User did not like this comment");
            }

            comment.LikeUsers.Remove(ApplicationUser.User);
            _context.SaveChanges();

            return Ok(comment.LikeUsers.Count);
        }
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        [HttpPost("{id}/dislikes")]
        public IActionResult Disike([FromRoute] int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
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

            if (comment.LikeUsers.Contains(ApplicationUser.User))
            {
                return BadRequest("User already disliked this comment");
            }

            comment.DislikeUsers.Add(ApplicationUser.User);
            _context.SaveChanges();

            return Ok(comment.DislikeUsers.Count);
        }

        [HttpDelete("{id}/dislikes")]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public IActionResult DelateDisike([FromRoute] int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound("Comment not found");
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

            if (!comment.LikeUsers.Contains(ApplicationUser.User))
            {
                return BadRequest("User did not dislike this comment");
            }

            comment.LikeUsers.Remove(ApplicationUser.User);
            _context.SaveChanges();

            return Ok(comment.LikeUsers.Count);
        }



    }


}
