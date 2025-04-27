namespace MagazinEAPI.Controllers
{
    using System.Security.Claims;
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Articles;
    using MagazinEAPI.Models.Users;
    using MagazinEAPI.Models.Users.Journalists;
    using MagazinEAPI.utils;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Controller for handling journalist-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JournalistController : ControllerBase
    {
        private readonly RolesBasedContext context;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="JournalistController"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userManager">Provides API for managing user in presistence store.</param>
        public JournalistController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Gets the list of journalists under the current head editor.
        /// </summary>
        /// <returns>in case of success returns collection of jouranlists id's.</returns>
        [HttpGet]
        [Authorize(Roles = "HeadEditor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ICollection<int>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
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

            var headEditor = await this.context.HeadEditors
                .Include(he => he.JournalistsUnder)
                .FirstOrDefaultAsync(he => he.ApplicationUserId == applicationUser.Id);

            return headEditor != null
                ? this.Ok(headEditor.JournalistsUnder.Select(j => j.Id).ToList())
                : this.NotFound("HeadEditor not found");
        }

        /// <summary>
        /// Gets the journalist by ID.
        /// Used by editors to get the journalist's info.
        /// </summary>
        /// <param name="id">Id of journalist.</param>
        /// <returns>dto of journalist object.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "HeadEditor, Editor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<JournalistDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var jouranlist = await this.context.Journalists.FirstOrDefaultAsync(j => j.Id == id);

            return jouranlist != null
                ? this.Ok(jouranlist.ToDTO())
                : this.NotFound("Journalist not found");
        }

        /// <summary>
        /// Creates a new journalist under the current head editor.
        /// </summary>
        /// <param name="email">email of the new journalist.</param>
        /// <returns>journalist dto of created journalist.</returns>
        [HttpPost("/{email}")]
        [Authorize(Roles = "HeadEditor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<JournalistDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromRoute] string email)
        {
            string? emailClaim = this.User.FindFirst(ClaimTypes.Email)?.Value;
            if (emailClaim == null)
            {
                return this.BadRequest("Head Editor's email not found");
            }

            var headEditor = await this.context.HeadEditors
                .Include(he => he.JournalistsUnder)
                .FirstOrDefaultAsync(he => he.ApplicationUser.Email == emailClaim);

            if (headEditor == null)
            {
                return this.NotFound("Head Editor not found");
            }

            if (headEditor.JournalistsUnder.Any(j => j.ApplicationUser.Email == email))
            {
                return this.BadRequest("Journalist already exists");
            }

            var userJournalist = await this.userManager.FindByEmailAsync(email);
            if (userJournalist == null)
            {
                return this.NotFound("User not found");
            }

            var journalist = new Journalist
            {
                ApplicationUserId = userJournalist.Id,
                ApplicationUser = userJournalist,
                HeadEditorId = headEditor.Id,
                HeadEditor = headEditor,
                Articles = new List<Article>(),
            };

            await this.context.Journalists.AddAsync(journalist);
            await this.context.SaveChangesAsync();

            return this.Ok(journalist.ToDTO());
        }

        /// <summary>
        /// Deletes the journalist by ID.
        /// </summary>
        /// <param name="id">Id of journalist.</param>
        /// <returns>info about success.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "HeadEditor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var journalist = await this.context.Journalists
                .Include(j => j.HeadEditor)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (journalist == null)
            {
                return this.NotFound("Journalist not found");
            }

            if (journalist.HeadEditorId != journalist.HeadEditor.Id)
            {
                return this.Unauthorized("You are not allowed to delete this journalist");
            }

            this.context.Journalists.Remove(journalist);
            await this.context.SaveChangesAsync();

            return this.Ok(journalist.ToDTO());
        }

        /// <summary>
        /// Gets the personal info of the journalist by ID.
        /// </summary>
        /// <param name="id">Id of journalist.</param>
        /// <returns>Dto of journalist object.</returns>
        [HttpGet("/{id}/PeronalInfos")]
        [Authorize(Roles = "HeadEditor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApplicationUserDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPersonalInfo([FromRoute] int id)
        {
            var journalist = await this.context.Journalists
                .Include(j => j.ApplicationUser)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (journalist == null)
            {
                return this.NotFound("Journalist not found");
            }

            var personalInfo = journalist.ApplicationUser.ToDTO();
            return this.Ok(personalInfo);
        }

    }
}
