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
        public IActionResult Post()
        {
            return Ok("Article created successfully");
        }
    }
}
