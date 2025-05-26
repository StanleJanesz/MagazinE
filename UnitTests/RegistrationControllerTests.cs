using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagazinEAPI.Contexts;
using MagazinEAPI.Controllers;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLibrary.DTO_Classes;
using Xunit;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.ComponentModel.DataAnnotations;

namespace UnitTests
{
	public class RegistrationControllerTests
	{
		private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
		private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
		private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
		private readonly Mock<ILogger<RegistrationController>> _mockLogger;
		private readonly Mock<RoleManager<ApplicationRole>> _mockRoleManager;
		private readonly RolesBasedContext _context;

		private readonly Mock<IUserEmailStore<ApplicationUser>> _mockEmailStore;

		public RegistrationControllerTests()
		{
			_mockEmailStore = new Mock<IUserEmailStore<ApplicationUser>>();

			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				_mockEmailStore.Object, null, null, null, null, null, null, null, null);
			_mockUserManager.SetupGet(x => x.SupportsUserEmail).Returns(true);


			_mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
				_mockUserManager.Object,
				Mock.Of<IHttpContextAccessor>(),
				Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
				null, null, null, null);

			_mockLogger = new Mock<ILogger<RegistrationController>>();
			_mockRoleManager = new Mock<RoleManager<ApplicationRole>>(
				Mock.Of<IRoleStore<ApplicationRole>>(), null, null, null, null);

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new RolesBasedContext(options);
		}

		[Fact]
		public async Task Post_ShouldRegisterUserSuccessfully()
		{
			var email = "newuser@example.com";
			var password = "Strong1!";
			var request = new RegisterRequestDTO
			{
				Email = email,
				Password = password,
				FirstName = "John",
				LastName = "Doe"
			};

			var user = new ApplicationUser { Email = email };

			_mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser)null);
			_mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), password)).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Reader")).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(m => m.GetUserIdAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("1");

			_mockEmailStore.Setup(m => m.SetUserNameAsync(It.IsAny<ApplicationUser>(), email, It.IsAny<CancellationToken>()))
						   .Returns(Task.CompletedTask);
			_mockEmailStore.Setup(m => m.SetEmailAsync(It.IsAny<ApplicationUser>(), email, It.IsAny<CancellationToken>()))
						   .Returns(Task.CompletedTask);

			var controller = new RegistrationController(
				_context,
				_mockUserManager.Object,
				_mockEmailStore.Object,
				_mockSignInManager.Object,
				_mockLogger.Object,
				_mockRoleManager.Object);

			var httpContext = new DefaultHttpContext();
			var routeData = new RouteData();
			var actionDescriptor = new ControllerActionDescriptor();

			controller.ControllerContext = new ControllerContext(
				new ActionContext(httpContext, routeData, actionDescriptor)
			);

			controller.Url = new UrlHelper(controller.ControllerContext);

			var result = await controller.Post(request);

			var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
			Xunit.Assert.Equal("User registered successfully", okResult.Value);
		}

		[Fact]
		public async Task Post_ShouldReturnBadRequest_WhenEmailExists()
		{
			var email = "existing@example.com";
			var request = new RegisterRequestDTO
			{
				Email = email,
				Password = "Strong1!",
				FirstName = "Test",
				LastName = "User"
			};

			var existingUser = new ApplicationUser { Email = email };
			_mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(existingUser);


			var controller = new RegistrationController(
				_context,
				_mockUserManager.Object,
				_mockEmailStore.Object,
				_mockSignInManager.Object,
				_mockLogger.Object,
				_mockRoleManager.Object);

			var httpContext = new DefaultHttpContext();
			var routeData = new RouteData();
			var actionDescriptor = new ControllerActionDescriptor();

			controller.ControllerContext = new ControllerContext(
				new ActionContext(httpContext, routeData, actionDescriptor)
			);

			controller.Url = new UrlHelper(controller.ControllerContext);

			var result = await controller.Post(request);

			var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);
			Xunit.Assert.Contains("We already have user with this email", badRequest.Value.ToString());
		}

		[Fact]
		public async Task Post_ShouldReturnBadRequest_WhenPasswordIsWeak()
		{
			var request = new RegisterRequestDTO
			{
				Email = "weak@example.com",
				Password = "weak",
				FirstName = "Weak",
				LastName = "Pass"
			};

			_mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
			_mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

			var controller = new RegistrationController(
				_context,
				_mockUserManager.Object,
				_mockEmailStore.Object,
				_mockSignInManager.Object,
				_mockLogger.Object,
				_mockRoleManager.Object);

			var httpContext = new DefaultHttpContext();
			var routeData = new RouteData();
			var actionDescriptor = new ControllerActionDescriptor();

			controller.ControllerContext = new ControllerContext(
				new ActionContext(httpContext, routeData, actionDescriptor)
			);

			controller.Url = new UrlHelper(controller.ControllerContext);


			ValidateModel(request, controller);

			var result = await controller.Post(request);

			var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);

			var errorsObj = badRequest.Value.GetType().GetProperty("errors")?.GetValue(badRequest.Value, null);
			var errors = errorsObj as IEnumerable<string>;

			Xunit.Assert.True(errors.Any(e => e.Contains("The Password must be")), "Expected an error message containing 'The Password must be'");
		}


		[Fact]
		public async Task Post_ShouldReturnBadRequest_WhenUserCreationFails()
		{
			var request = new RegisterRequestDTO
			{
				Email = "failuser@example.com",
				Password = "Strong1!",
				FirstName = "Fail",
				LastName = "User"
			};

			_mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
			_mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Unexpected error" }));

			var controller = new RegistrationController(
				_context,
				_mockUserManager.Object,
				_mockEmailStore.Object,
				_mockSignInManager.Object,
				_mockLogger.Object,
				_mockRoleManager.Object);

			var httpContext = new DefaultHttpContext();
			var routeData = new RouteData();
			var actionDescriptor = new ControllerActionDescriptor();

			controller.ControllerContext = new ControllerContext(
				new ActionContext(httpContext, routeData, actionDescriptor)
			);

			controller.Url = new UrlHelper(controller.ControllerContext);

			var result = await controller.Post(request);

			var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);

			var errorsProperty = badRequest.Value.GetType().GetProperty("errors");
			Xunit.Assert.NotNull(errorsProperty);

			var errorsValue = errorsProperty.GetValue(badRequest.Value) as IEnumerable<object>;
			Xunit.Assert.NotNull(errorsValue);

			Xunit.Assert.Contains(errorsValue, e => e.ToString().Contains("Unexpected error"));
		}

		[Fact]
		public async Task Post_ShouldReturnBadRequest_WhenEmailIsEmpty()
		{
			var request = new RegisterRequestDTO
			{
				Email = "",
				Password = "Strong1!",
				FirstName = "Empty",
				LastName = "Email"
			};

			_mockUserManager.Setup(m => m.FindByEmailAsync("")).ReturnsAsync((ApplicationUser)null);
			_mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email is required" }));

			_mockUserManager.Setup(m => m.GetUserIdAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("1");
			_mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Reader"))
				.ReturnsAsync(IdentityResult.Success);

			_mockEmailStore.Setup(m => m.SetUserNameAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			_mockEmailStore.Setup(m => m.SetEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var controller = new RegistrationController(
				_context,
				_mockUserManager.Object,
				_mockEmailStore.Object,
				_mockSignInManager.Object,
				_mockLogger.Object,
				_mockRoleManager.Object);

			var httpContext = new DefaultHttpContext();
			var routeData = new RouteData();
			var actionDescriptor = new ControllerActionDescriptor();

			controller.ControllerContext = new ControllerContext(
				new ActionContext(httpContext, routeData, actionDescriptor)
			);

			controller.Url = new UrlHelper(controller.ControllerContext);

			ValidateModel(request, controller);

			var result = await controller.Post(request);

			var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);

			var errorsObj = badRequest.Value.GetType().GetProperty("errors")?.GetValue(badRequest.Value, null);
			var errors = errorsObj as IEnumerable<string>;

			Xunit.Assert.Contains("The Email field is required.", errors);
		}



		private static void ValidateModel(object model, ControllerBase controller)
		{
			var validationContext = new ValidationContext(model, null, null);
			var validationResults = new List<ValidationResult>();
			Validator.TryValidateObject(model, validationContext, validationResults, true);

			foreach (var validationResult in validationResults)
			{
				foreach (var memberName in validationResult.MemberNames)
				{
					controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
				}
			}
		}

	}

}
