namespace MagazinEAPI.Controllers
{
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Articles;
    using MagazinEAPI.Models.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Controller for managing tags.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly RolesBasedContext context;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsController"/> class.
        /// Constructor for TagsController.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userManager">Provides API for managing user in presistence store.</param>
        public TagsController(RolesBasedContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }


        /// <summary>
        /// Gets one tag by its ID.
        /// </summary>
        /// <param name="id">id of tag.</param>
        /// <returns>tagDTO.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<TagDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var tag = await this.context.Tags.FindAsync(id);
            if (tag == null)
            {
                return this.NotFound();
            }
            var tagDTO = tag.ToDTO();

            return this.Ok(tagDTO);
        }

        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns>tagDTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<TagDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var tags = await this.context.Tags.ToListAsync();
            if (tags == null || tags.Count == 0)
            {
                return this.NotFound();
            }

            var tagsDTOs = tags.Select(t => t.ToDTO()).ToList();

            return this.Ok(tagsDTOs);
        }


        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="tagDTO">dto containg name of added tag.</param>
        /// <returns>returns DTO of added tag.</returns>
        [HttpPost]
        [Authorize(Roles = "HeadEditor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<TagDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] TagDTO tagDTO)
        {
            if (tagDTO == null)
            {
                return this.BadRequest();
            }

            var tag = new Tag
            {
                Name = tagDTO.Name,
            };

            await this.context.Tags.AddAsync(tag);
            await this.context.SaveChangesAsync();

            return this.Ok(tag.ToDTO());
        }


        /// <summary>
        /// removes a tag.
        /// </summary>
        /// <param name="id">id of removed tag.</param>
        /// <returns>dto of removed tag.</returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<TagDTO>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var tag = await this.context.Tags.FindAsync(id);
            if (tag == null)
            {
                return this.NotFound();
            }

            this.context.Tags.Remove(tag);
            await this.context.SaveChangesAsync();

            return this.Ok(tag.ToDTO());
        }

    }
