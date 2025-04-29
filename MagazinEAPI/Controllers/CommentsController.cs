namespace MagazinEAPI.Controllers
{
    using System.Security.Claims;
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Articles.Comment;
    using MagazinEAPI.Models.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Controller for managing comments.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly RolesBasedContext context;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsController"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userManager">Provides API for managing user in presistence store.</param>
        public CommentsController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Gets a comment by its ID.
        /// </summary>
        /// <param name="id">Id of requested comment.</param>
        /// <returns>In case of success returns dto of comment object.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<CommentDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var comment = await this.context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            if (!comment.Article.isPublished)
            {
                return this.Unauthorized("Comment cannot be viewed because article is not published");
            }

            if (comment.IsDeleted && !this.User.IsInRole("Admin"))
            {
                return this.Unauthorized("Comment cannot be viewed because it is deleted");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.BadRequest("User not found");
            }

            if (!comment.Article.CanBeViewedBy(applicationUser, this.context, this.userManager))
            {
                return this.Unauthorized("User cannot view this comment");
            }

            return this.Ok(comment.ToDTO());
        }

        /// <summary>
        /// Adds a new comment to the article.
        /// </summary>
        /// <param name="commentDTO">dto object with comment info.</param>
        /// <returns>In case of success returns dto of added comment.</returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CommentDTO commentDTO)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null || applicationUser.User == null)
            {
                return this.BadRequest("User not found");
            }

            var article = await this.context.Articles.FirstOrDefaultAsync(a => a.Id == commentDTO.ArticleId);
            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            if (!article.CanBeViewedBy(applicationUser, this.context, this.userManager))
            {
                return this.Unauthorized("User cannot view this article");
            }

            var comment = new Comment();
            comment.UpdateFromDTO(commentDTO);

            if (commentDTO.ParentId != null)
            {
                var parent = await this.context.Comments.FirstOrDefaultAsync(c => c.Id == commentDTO.ParentId);
                if (parent == null)
                {
                    return this.NotFound("Parent comment not found");
                }

                comment.Parent = parent;
            }

            comment.Author = applicationUser.User;
            comment.Article = article;
            comment.AuthorId = applicationUser.User.Id;

            this.context.Add(comment);
            this.context.SaveChanges();
            this.context.Comments.Add(comment);
            this.context.SaveChanges();

            return this.Ok(comment.ToDTO());
        }

        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="id">Id of comment to be delated.</param>
        /// <returns>response with info about success.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await this.context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.BadRequest("User not found");
            }

            comment.IsDeleted = true;
            comment.DeletedBy = applicationUser.Admin;
            this.context.SaveChanges();

            return this.Ok();
        }

        /// <summary>
        /// Restores a deleted comment by its ID.
        /// </summary>
        /// <param name="id">Id of comment to be restored.</param>
        /// <returns>response with info about success.</returns>
        [HttpPut("{id}/restore")]
        [Authorize(Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Restore(int id)
        {
            var comment = await this.context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            comment.IsDeleted = false;
            comment.DeletedBy = null;
            this.context.SaveChanges();

            return this.Ok();
        }

        /// <summary>
        /// Changes content of comment.
        /// </summary>
        /// <param name="id">Id of changed comment.</param>
        /// <param name="commentDTO">Dto containing comment changes.</param>
        /// <returns>In case of succes dto of changed comment.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<CommentDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(int id, [FromBody] CommentDTO commentDTO)
        {
            var comment = this.context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null || applicationUser.User == null)
            {
                return this.BadRequest("User not found");
            }

            if (comment.AuthorId != applicationUser.User.Id)
            {
                return this.Unauthorized("User can only edit his comments");
            }

            comment.Content = commentDTO.Content;

            await this.context.SaveChangesAsync();

            return this.Ok(comment.ToDTO());
        }

        /// <summary>
        /// Adds a like to the comment.
        /// </summary>
        /// <param name="id">Id of liked comment.</param>
        /// <returns>count of likes in comment.</returns>
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        [HttpPost("{id}/likes")]
        public async Task<IActionResult> Like([FromRoute] int id)
        {
            var comment = await this.context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null || applicationUser.User == null)
            {
                return this.BadRequest("User not found");
            }

            if (comment.LikeUsers.Contains(applicationUser.User))
            {
                return this.BadRequest("User already liked this comment");
            }

            comment.LikeUsers.Add(applicationUser.User);
            await this.context.SaveChangesAsync();

            return this.Ok(comment.LikeUsers.Count);
        }

        /// <summary>
        /// Removes a like from the comment.
        /// </summary>
        /// <param name="id">Id of comment.</param>
        /// <returns>Count od likes in comment.</returns>
        [HttpDelete("{id}/likes")]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> DelateLike([FromRoute] int id)
        {
            var comment = this.context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null || applicationUser.User == null)
            {
                return this.BadRequest("User not found");
            }

            if (!comment.LikeUsers.Contains(applicationUser.User))
            {
                return this.BadRequest("User did not like this comment");
            }

            comment.LikeUsers.Remove(applicationUser.User);
            await this.context.SaveChangesAsync();

            return this.Ok(comment.LikeUsers.Count);
        }

        /// <summary>
        /// Adds a dislike to the comment.
        /// </summary>
        /// <param name="id">Id of the comment.</param>
        /// <returns>Count of dislikes for comment.</returns>
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        [HttpPost("{id}/dislikes")]
        public async Task<IActionResult> Disike([FromRoute] int id)
        {
            var comment = await this.context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null || applicationUser.User == null)
            {
                return this.BadRequest("User not found");
            }

            if (comment.LikeUsers.Contains(applicationUser.User))
            {
                return this.BadRequest("User already disliked this comment");
            }

            comment.DislikeUsers.Add(applicationUser.User);
            await this.context.SaveChangesAsync();

            return this.Ok(comment.DislikeUsers.Count);
        }

        /// <summary>
        /// Removes a dislike from the comment.
        /// </summary>
        /// <param name="id">Id of the comment.</param>
        /// <returns>Count of dislikes for this comment.</returns>
        [HttpDelete("{id}/dislikes")]
        [Authorize(Roles = "User")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> DelateDisike([FromRoute] int id)
        {
            var comment = this.context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound("Comment not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Value);
            if (applicationUser == null || applicationUser.User == null)
            {
                return this.BadRequest("User not found");
            }

            if (!comment.LikeUsers.Contains(applicationUser.User))
            {
                return this.BadRequest("User did not dislike this comment");
            }

            comment.LikeUsers.Remove(applicationUser.User);
            await this.context.SaveChangesAsync();

            return this.Ok(comment.LikeUsers.Count);
        }
    }
}
