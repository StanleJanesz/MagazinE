using MagazinE.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MagazinEAPI.Controllers
{
    [ApiController]
    [Route("articles")]
    public class AtricleController : Controller
    {
        [HttpGet(Name = "GetArticle")]
        public IActionResult Get()
        {
            return Ok("Article 1");
        }

        [HttpPost(Name = "CreateArticle")]
        public IActionResult Post(string Title, string authorEmail)
        {
            if (Title == null || authorEmail == null)
            {
                return BadRequest("Title and author are required");
            }

            //  TODO: zdobycie id autora
            int authorId = 1;

            //  TODO: sprawdzanie czy autor istnieje
            if (false)
            {
                return BadRequest("Author does not exist");
            }

            // TODO: sprawdzanie czy tytuł jest unikalny
            if (false)
            {
                return BadRequest("Title must be unique");
            }


            Article article = new Article(Title, authorId);

            // TODO: zapisanie artykułu w bazie danych
            return Ok("Article created successfully");
        }
    }
}
