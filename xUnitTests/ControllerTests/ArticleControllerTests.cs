using MagazinEAPI.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagazinEAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace xUnitTests.ControllerTests
{
    public class ArticleControllerTests
    {
        [Fact]
        public async Task nullRequest()
        {
            var controller = new AtricleController();
            controller.ModelState.AddModelError("SessionName", "Required");


            var result = await controller.Post("", "");
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task wrongEmailRequest()
        {
            var controller = new AtricleController();
            controller.ModelState.AddModelError("SessionName", "Required");


            var result = await controller.Post("Title", "em.adfadf.adfadfa");
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
