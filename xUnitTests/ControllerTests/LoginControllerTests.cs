using MagazinEAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagazinEAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace xUnitTests.ControllerTests
{
    public class LoginControllerTests
    {
        [Fact]
        public void LoginWithGoogleTest()
        {

            //var controller = new LoginController();
            //controller.ModelState.AddModelError("SessionName", "Required");
            

            //var result = controller.LoginWithGoogle();
            //Assert.IsType<BadRequest>(result);
        }

        [Fact]
        public async Task GoogleResponseTest()
        {
            //var controller = new LoginController();
            //controller.ModelState.AddModelError("SessionName", "Required");
            

            //var result = await controller.GoogleResponse();
            //Assert.IsType<BadRequest>(result);
        }
    }
}
