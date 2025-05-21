using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users.Editors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTO_Classes;

namespace MagazinEAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EditorsController : ControllerBase
    {
        private readonly RolesBasedContext _RolesBasedContext;

        public EditorsController(RolesBasedContext RolesBasedContext)
        {
            _RolesBasedContext = RolesBasedContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetEditors()
        {
            var editors =  _RolesBasedContext.Editors.ToList();
            if (editors == null || !editors.Any())
            {
                return NotFound("No editors found.");
            }
            return Ok(editors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEditor(string id)
        {
            var editor = await _RolesBasedContext.Editors.FindAsync(id);
            if (editor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }
            return Ok(editor);
        }

        [HttpGet("/articles/{id}")]
        public async Task<IActionResult> GetEditorArticles(string id)
        {
            var editor = await _RolesBasedContext.Editors.FindAsync(id);
            if (editor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }

            var articles =  _RolesBasedContext.Articles
                .Where(a => a.Tags.Any(x => editor.AllowedTags.Contains(x)))
                .ToList();
            return Ok(articles);
        }


        [HttpPost]
        public async Task<IActionResult> CreateEditor([FromBody] Editor editor)
        {
            if (editor == null)
            {
                return BadRequest("Editor data is null.");
            }
            await _RolesBasedContext.Editors.AddAsync(editor);
            _RolesBasedContext.SaveChanges();
            return CreatedAtAction("create", new { id = editor.Id }, editor);
        }
        [HttpPost("{id}/tags")]
        public async Task<IActionResult> AddEditorTags([FromQuery] string id, [FromBody] TagDTO tag)
        {
            var editor = await _RolesBasedContext.Editors.FindAsync(id);
            if (editor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }
            if (tag == null)
            {
                return BadRequest("Tag data is null.");
            }
            var newTag = new Tag
            {
                Id = tag.Id,
                Name = tag.Name,
            };
            newTag.Editors.Add(editor);
            editor.AllowedTags.Add(newTag);
            await _RolesBasedContext.SaveChangesAsync();
            return CreatedAtAction("create", new { id = tag.Id }, tag);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEditor([FromQuery] string id, [FromBody] Editor editor)
        {
            if (editor == null || int.Parse(id) != editor.Id)
            {
                return BadRequest("Editor data is null or ID mismatch.");
            }
            var existingEditor = await _RolesBasedContext.Editors.FindAsync(id);
            if (existingEditor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }           
            existingEditor.HeadEditorId = editor.HeadEditorId;
            existingEditor.AllowedTags = editor.AllowedTags;
            _RolesBasedContext.Editors.Update(existingEditor);
            await _RolesBasedContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEditor(string id)
        {
            var editor = await _RolesBasedContext.Editors.FindAsync(id);
            if (editor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }
            _RolesBasedContext.Editors.Remove(editor);
            await _RolesBasedContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("/articles/{id}")]
        public async Task<IActionResult> DeleteEditorArticles([FromQuery] string id)
        {
            var editor = await _RolesBasedContext.Editors.FindAsync(id);
            if (editor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }
            var article = await _RolesBasedContext.Articles.FindAsync(id);

            if(article == null)
            {
                return NotFound($"Article with ID {id} not found.");
            }

            _RolesBasedContext.Articles.Remove(article);
            await _RolesBasedContext.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}/tags")]
        public async Task<IActionResult> DeleteEditorTags([FromQuery] string id)
        {
            var editor = await _RolesBasedContext.Editors.FindAsync(id);
            if (editor == null)
            {
                return NotFound($"Editor with ID {id} not found.");
            }
            var tag = await _RolesBasedContext.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound($"Tag with ID {id} not found.");
            }
            editor.AllowedTags.Remove(tag);
            await _RolesBasedContext.SaveChangesAsync();
            return Ok();
        }
    }
}
