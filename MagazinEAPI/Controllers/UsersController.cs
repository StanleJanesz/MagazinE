namespace MagazinEAPI.Controllers
{
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SharedLibrary.DTO_Classes;
    using System.Security.Claims;

    /// <summary>
    /// Controller for managing user-related operations.
    /// </summary>
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly RolesBasedContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Gets the current user's information.
        /// </summary>
        /// <returns>Logged in user's info</returns>
        [HttpGet]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            return this.Ok(applicationUser.User.ToDTO());
        }

        /// <summary>
        /// Gets the user's information.
        /// Used by admin.
        /// </summary>
        /// <param name="id">Id of the user.</param>
        /// <returns>Logged in user's info.</returns>
        [HttpGet("/{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult Get([FromRoute] string id)
        {
            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Id == id);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            return this.Ok(applicationUser.User.ToDTO());
        }

        /// <summary>
        /// Adds an article to the user's "to read" list.
        /// </summary>
        /// <param name="id">Id of the article</param>
        /// <returns>dto object with user info and updated list.</returns>
        [HttpPost("/ToRead/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult Put([FromRoute] int id)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var article = this.context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            try
            {
                applicationUser.User.ArticleToReadArticles.Add(article);

                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var userDTO = applicationUser.User.ToDTO();

            return this.Ok(userDTO);
        }

        /// <summary>
        /// Deletes an article from the user's "to read" list.
        /// </summary>
        /// <param name="id">id of article.</param>
        /// <returns>dto object with user info and updated list.</returns>
        [HttpDelete("/ToRead/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult DeleteToRead([FromRoute] int id)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var article = this.context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            try
            {
                if (!applicationUser.User.ArticleToReadArticles.Remove(article))
                {
                    return this.BadRequest("Article is not in the to read list");
                }

                this.context.SaveChanges();

            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var userDTO = applicationUser.User.ToDTO();

            return this.Ok(userDTO);
        }

        /// <summary>
        /// Deletes an article from the user's favorite list.
        /// </summary>
        /// <param name="id">Id of the article.</param>
        /// <returns>dto object with user info and updated list.</returns>
        [HttpDelete("/Favorite/Articles/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult DeleteFavorite([FromRoute] int id)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var article = this.context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            try
            {
                if (!applicationUser.User.ArticleFavoriteArticles.Remove(article))
                {
                    return this.BadRequest("Article is not in the to favorite list");
                }

                this.context.SaveChanges();

            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var userDTO = applicationUser.User.ToDTO();

            return this.Ok(userDTO);
        }

        /// <summary>
        /// Adds an article to the user's favorite list.
        /// </summary>
        /// <param name="id">Id of the article.</param>
        /// <returns>dto object with user info and updated list.</returns>
        [HttpPut("/Favorite/Articles/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult PutFavoriteArticle([FromRoute] int id)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var article = this.context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return this.NotFound("Article not found");
            }

            try
            {
                applicationUser.User.ArticleFavoriteArticles.Add(article);

                this.context.SaveChanges();

            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var userDTO = applicationUser.User.ToDTO();

            return this.Ok(userDTO);
        }

        /// <summary>
        /// Adds a tag to the user's favorite list.
        /// </summary>
        /// <param name="id">Tag id.</param>
        /// <returns>dto object with user info and updated list.</returns>
        [HttpPut("/Favorite/Tags/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult PutFavoriteTag([FromRoute] int id)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var tag = this.context.Tags.FirstOrDefault(u => u.Id == id);

            if (tag == null)
            {
                return this.NotFound("Tag not found");
            }

            try
            {
                applicationUser.User.FavouriteTags.Add(tag);

                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var userDTO = applicationUser.User.ToDTO();

            return this.Ok(userDTO);
        }

        /// <summary>
        /// Deletes a tag from the user's favorite list.
        /// </summary>
        /// <param name="id">Id of the tag.</param>
        /// <returns>dto object with user info and updated list.</returns>
        [HttpDelete("/Favorite/Tags/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult DeleteFavoriteTag([FromRoute] int id)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var tag = this.context.Tags.FirstOrDefault(u => u.Id == id);

            if (tag == null)
            {
                return this.NotFound("Tag not found");
            }

            try
            {
                if (applicationUser.User.FavouriteTags.Remove(tag))
                {
                    return this.BadRequest("Tag is not in the favorite tags list");
                }

                this.context.SaveChanges();

            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var userDTO = applicationUser.User.ToDTO();

            return this.Ok(userDTO);
        }

        /// <summary>
        /// Gets the list of articles owned by the user.
        /// </summary>
        /// <returns>Collection of id's of owned articles.</returns>
        [HttpGet("/OwnedArticles")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ICollection<int>>(StatusCodes.Status200OK)]
        public IActionResult GetOwnedArticles()
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            if (applicationUser.User == null)
            {
                return this.NotFound("User not found");
            }

            var ownedArticles = applicationUser.User.OwnedArticles.Select( a => a.Id);

            return this.Ok(ownedArticles);
        }

        /// <summary>
        /// Gets the personal information of the user.
        /// </summary>
        /// <returns>dto with user informations.</returns>
        [HttpGet("/PersonalInfo")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApplicationUserDTO>(StatusCodes.Status200OK)]
        public IActionResult GetPersonalInfo()
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            return this.Ok(applicationUser.ToDTO());
        }

        /// <summary>
        /// Gets the personal information of the user.
        /// used by admin.
        /// </summary>
        /// <returns>dto with user informations.</returns>
        [HttpGet("/PersonalInfo/{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApplicationUserDTO>(StatusCodes.Status200OK)]
        public IActionResult GetPersonalInfo([FromRoute] int id)
        {
            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.User.Id == id );
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            return this.Ok(applicationUser.ToDTO());
        }

        /// <summary>
        /// Updates the personal information of the user.
        /// </summary>
        /// <param name="appUserDTO">dto wih user info to be updated.</param>
        /// <returns>dto object with updated user info.</returns>
        [HttpPut("/PersonalInfo")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApplicationUserDTO>(StatusCodes.Status200OK)]
        public IActionResult PutPersonalInfo([FromBody] ApplicationUserDTO appUserDTO)
        {
            var email = this.User.FindFirst(ClaimTypes.Email);
            if (email == null)
            {
                return this.BadRequest("Email not found");
            }

            var applicationUser = this.userManager.Users.FirstOrDefault(u => u.Email == email.Value);
            if (applicationUser == null)
            {
                return this.NotFound("User not found");
            }

            applicationUser.FirstName = appUserDTO.FirstName;
            applicationUser.LastName = appUserDTO.LastName;
            applicationUser.UserName = appUserDTO.Login;

            this.context.SaveChanges();

            return this.Ok(applicationUser.ToDTO());
        }

    }
}
