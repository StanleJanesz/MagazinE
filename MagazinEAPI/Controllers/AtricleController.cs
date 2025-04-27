namespace MagazinEAPI.Controllers
{
    using System.Security.Claims;
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Articles;
    using MagazinEAPI.Models.Users;
    using MagazinEAPI.utils;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Controller for managing articles.
    /// </summary>
    [ApiController]
    [Route("articles")]
    public class AtricleController : Controller
    {
        private readonly RolesBasedContext context;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtricleController"/> class.
        /// Constructor for AtricleController.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userManager">Provides API for managing user in presistence store.</param>
        public AtricleController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Gets the article by id.
        /// </summary>
        /// <param name="id">Id of requested article.</param>
        /// <returns>Context based response.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        [ProducesResponseType<ArticleDTO>(StatusCodes.Status200OK)]
        public IActionResult Get([FromRoute] int id)
        {
            var article = this.context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.BadRequest("User not found");
            }

            if (!article.CanBeViewedBy(applicationUser, this.context, this.userManager))
            {
                return this.Unauthorized("User cannot view this article");
            }

            return this.Ok(article.ToDTO());
        }

        /// <summary>
        /// Changes the article.
        /// For Journalist and Editor roles.
        /// </summary>
        /// <param name="id">Id of changed article.</param>
        /// <param name="articleDTO">Dto for givne changes.</param>
        /// <returns>Context based response.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Journalist, Editor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Put([FromRoute] int id, [FromBody] ArticleDTO articleDTO)
        {
            var article = this.context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.BadRequest("User not found");
            }

            if (!article.CanBeEditedBy(applicationUser, this.context, this.userManager))
            {
                return this.Unauthorized("User cannot edit this article");
            }

            if (articleDTO == null)
            {
                return this.BadRequest("Request is empty");
            }

            try
            {
                article.Title = articleDTO.Title;
                article.Content = articleDTO.Content;
                article.Introduction = articleDTO.Introduction;

                // TODO: deciede how we want to handle photos
                // TODO: tags should be in another call (editor can not change tags?  only author and headEditor ?)
                // TODO: publish and premium should be in another call (only Editor HeadEditor but not Author)
                this.context.Articles.Update(article);
                this.context.SaveChanges();
            }
            catch
            {
                return this.BadRequest("Editing article failed");
            }

            return this.Ok();
        }

        /// <summary>
        /// Deletes the article.
        /// Available for Journalist, Editor and HeadEditor roles.
        /// </summary>
        /// <param name="id">Id of delated article.</param>
        /// <returns>Context based response.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Journalist, Editor, HeadEditor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Delete([FromRoute] int id)
        {
            var article = this.context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.BadRequest("User not found");
            }

            if (!article.CanBeDelatedBy(applicationUser, this.context, this.userManager))
            {
                return this.Unauthorized("User cannot delate this article");
            }

            try
            {
                this.context.Articles.Remove(article);
                this.context.SaveChanges();
            }
            catch
            {
                return this.BadRequest("Deleting article failed");
            }

            return this.Ok();
        }

        /// <summary>
        /// Gets the articles for main page.
        /// Not authorized becouse it is designed to get articles for main page.
        /// </summary>
        /// <param name="articlesRequestDTO">Filters for requested articles batch.</param>
        /// <returns>Collection of article dto objects.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ICollection<ArticleDTO>>(StatusCodes.Status200OK)]
        public IActionResult Get([FromQuery]ArticlesRequestDTO articlesRequestDTO) // TODO: decide what kind of filter will be used and which orders will be available
        {
            if (articlesRequestDTO == null)
            {
                return this.BadRequest("Request is empty");
            }

            int skip = articlesRequestDTO.BatchSize * articlesRequestDTO.Page;

            List<Article> articles;

            try // Ugly code, to be refactored
            {
                articles = this.context.Articles
                    .Where(a => a.isPublished)

                   // .Where( a => articlesRequestDTO.Tags.All(t => a.Tags.Any(tag => tag.Id == t))) // dont know what kinds of filters will be used
                   // .Where(a => articlesRequestDTO.Authors.Any(author => a.Author.Id == author))  // dont know what kinds of filters will be used
                   .OrderBy(a => a.TimeOfPublication) // TODO: decide which order will be default
                   .Skip(skip)
                   .Take(articlesRequestDTO.BatchSize)
                   .ToList();
            }
            catch
            {
                return this.BadRequest("Getting articles failed");
            }

            return this.Ok(articles.Select(a => a.ToInfoDTO()));
        }

        /// <summary>
        /// Creates a new article.
        /// For Journalist role.
        /// </summary>
        /// <param name="articleDTO">Dto object with article info.</param>
        /// <returns>Context based response.</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Journalist")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromQuery]ArticleDTO articleDTO)

        // content can be empty will be added later,Title is oblligatory,  email of author is taken from token
        // also could consider adding from only string title as other can be included later using put
        // i would also consider making content and introduction optional
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (articleDTO == null || articleDTO.Title == null)
            {
                return this.BadRequest("Title is required");
            }

            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var user = await this.userManager.FindByEmailAsync(email.Value);
            if (user == null)
            {
                return this.BadRequest("User not found");
            }

            var journalist = this.context.Journalists.FirstOrDefault(j => j.ApplicationUserId == user.Id);
            if (journalist == null)
            {
                return this.Unauthorized("User is not a journalist");
            }

            if (this.context.Articles.Any(a => a.Title == articleDTO.Title)) // TODO: we have to decide if we want to allow articles with the same title
            {
                return this.BadRequest("Article with this title already exists");
            }

            try
            {
                this.context.Articles.Add(new Article
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
                this.context.SaveChanges();
            }
            catch
            {
                return this.BadRequest("Adding article failed");
            }

            return this.Created();
        }
    }
}
