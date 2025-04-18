using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Readers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTO_Classes;
using System.Security.Claims;


namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {

        private readonly RolesBasedContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            return Ok(ApplicationUser.User.ToDTO());
        }

        [HttpPost("/ToRead/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult Put([FromRoute] int id)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var article = _context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return NotFound("Article not found");
            }

            try
            {


                ApplicationUser.User.ArticleToReadArticles.Add(article);

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var userDTO = ApplicationUser.User.ToDTO();

            return Ok(userDTO);
        }
        [HttpDelete("/ToRead/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult DeleteToRead([FromRoute] int id)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var article = _context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return NotFound("Article not found");
            }

            try
            {
                if (!ApplicationUser.User.ArticleToReadArticles.Remove(article))
                {
                    return BadRequest("Article is not in the to read list");
                }

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var userDTO = ApplicationUser.User.ToDTO();


            return Ok(userDTO);
        }

        [HttpDelete("/Favorite/Articles/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult DeleteFavorite([FromRoute] int id)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var article = _context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return NotFound("Article not found");
            }

            try
            {
                if (!ApplicationUser.User.ArticleFavoriteArticles.Remove(article))
                {
                    return BadRequest("Article is not in the to favorite list");
                }

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var userDTO = ApplicationUser.User.ToDTO();


            return Ok(userDTO);
        }
        
        [HttpPut("/Favorite/Articles/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult PutFavoriteArticle([FromRoute] int id)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var article = _context.Articles.FirstOrDefault(u => u.Id == id);

            if (article == null)
            {
                return NotFound("Article not found");
            }

            try
            {
                ApplicationUser.User.ArticleFavoriteArticles.Add(article);

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var userDTO = ApplicationUser.User.ToDTO();


            return Ok(userDTO);
        }

        [HttpPut("/Favorite/Tags/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult PutFavoriteTag([FromRoute] int id)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var tag = _context.Tags.FirstOrDefault(u => u.Id == id);

            if (tag == null)
            {
                return NotFound("Tag not found");
            }

            try
            {
                ApplicationUser.User.FavouriteTags.Add(tag);

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var userDTO = ApplicationUser.User.ToDTO();


            return Ok(userDTO);
        }

        [HttpDelete("/Favorite/Tags/{id}")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
        public IActionResult DeleteFavoriteTag([FromRoute] int id)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var tag = _context.Tags.FirstOrDefault(u => u.Id == id);

            if (tag == null)
            {
                return NotFound("Tag not found");
            }

            try
            {
                if (ApplicationUser.User.FavouriteTags.Remove(tag))
                {
                    return BadRequest("Tag is not in the favorite tags list");
                }

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var userDTO = ApplicationUser.User.ToDTO();


            return Ok(userDTO);
        }
        [HttpGet("/OwnedArticles")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ICollection<int>>(StatusCodes.Status200OK)]
        public IActionResult GetOwnedArticles()
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            if (ApplicationUser.User == null)
            {
                return NotFound("User not found");
            }

            var ownedArticles = ApplicationUser.User.OwnedArticles.Select( a => a.Id);


            return Ok(ownedArticles);
        }

        [HttpGet("/PersonalInfo")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApplicationUserDTO>(StatusCodes.Status200OK)]
        public IActionResult GetPersonalInfo()
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }


            return Ok(ApplicationUser.ToDTO());
        }

        [HttpPut("/PersonalInfo")]
        [Authorize(Roles = "Reader")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApplicationUserDTO>(StatusCodes.Status200OK)]
        public IActionResult PutPersonalInfo([FromBody] ApplicationUserDTO appUserDTO)
        {
            var Email = User.FindFirst(ClaimTypes.Email);
            if (Email == null)
            {
                return BadRequest("Email not found");
            }

            var ApplicationUser = _userManager.Users.FirstOrDefault(u => u.Email == Email.Value);
            if (ApplicationUser == null)
            {
                return NotFound("User not found");
            }

            ApplicationUser.FirstName = appUserDTO.FirstName;
            ApplicationUser.LastName = appUserDTO.LastName;
            ApplicationUser.UserName = appUserDTO.Login;

            _context.SaveChanges();


            return Ok(ApplicationUser.ToDTO());
        }

    }
}
