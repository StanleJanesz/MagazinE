using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MagazinEAPI.Controllers;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users;
using SharedLibrary.DTO_Classes;
using MagazinEAPI.Models.Users.Journalists;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace UnitTests
{
	public class ArticleControllerTestsAddDelete
	{
		[Fact]
		public async Task Post_ShouldReturnCreated_WhenValidJournalist()
		{
			var email = "journalist@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };
			var journalist = new Journalist { Id = 1, ApplicationUserId = "1" };

			var articleDTO = new ArticleDTO
			{
				Title = "Test Article",
				Content = "This is a test content.",
				Introduction = "This is a test introduction."
			};

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			var articles = new List<Article>();
			var mockArticleSet = MockDbSetHelper.CreateMockDbSet(articles);
			var mockJournalists = MockDbSetHelper.CreateMockDbSet(new List<Journalist> { journalist });
			var mockApplicationUsers = MockDbSetHelper.CreateMockDbSet(new List<ApplicationUser> { user });

			var mockContext = new Mock<RolesBasedContext>(new DbContextOptions<RolesBasedContext>());
			mockContext.Setup(c => c.Articles).Returns(mockArticleSet.Object);
			mockContext.Setup(c => c.Journalists).Returns(mockJournalists.Object);
			mockContext.Setup(c => c.Users).Returns(mockApplicationUsers.Object);

			var controller = new ArticleController(mockContext.Object, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.Role, "Journalist")
				}))
				}
			};

			var result = await controller.Post(articleDTO);
			var createdResult = Xunit.Assert.IsType<CreatedResult>(result);
			Xunit.Assert.Equal(201, createdResult.StatusCode);
		}

		[Fact]
		public async Task Post_ShouldReturnCreated_WhenValidJournalist2()
		{
			var email = "journalist@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };
			var journalist = new Journalist { Id = 1, ApplicationUserId = "1" };

			var articleDTO = new ArticleDTO
			{
				Title = "Test Article",
				Content = "This is a test content.",
				Introduction = "This is a test introduction."
			};

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new RolesBasedContext(options);
			context.Journalists.Add(journalist);
			await context.SaveChangesAsync();

			var controller = new ArticleController(context, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.Role, "Journalist")
				}))
				}
			};

			var result = await controller.Post(articleDTO);
			var createdResult = Xunit.Assert.IsType<CreatedResult>(result);
			Xunit.Assert.Equal(201, createdResult.StatusCode);

			var articleCount = await context.Articles.CountAsync();
			Xunit.Assert.Equal(1, articleCount);
		}

		[Fact]
		public async Task Post_ShouldReturnUnauthorized_WhenUserIsNotJournalist()
		{
			var email = "user@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };

			var articleDTO = new ArticleDTO
			{
				Title = "Duplicate Title",
				Content = "Content",
				Introduction = "Intro"
			};

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new RolesBasedContext(options); // Brak dziennikarza

			var controller = new ArticleController(context, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
				new Claim(ClaimTypes.Email, email),
				new Claim(ClaimTypes.Role, "User") // nie "Journalist"
			}))
				}
			};

			var result = await controller.Post(articleDTO);
			var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
			Xunit.Assert.Equal(401, unauthorizedResult.StatusCode);
		}

		[Fact]
		public async Task Post_ShouldReturnBadRequest_WhenTitleAlreadyExists()
		{
			var email = "journalist@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };
			var journalist = new Journalist { Id = 1, ApplicationUserId = user.Id };

			var articleDTO = new ArticleDTO
			{
				Title = "Duplicate Title",
				Content = "New content",
				Introduction = "Intro"
			};

			var existingArticle = new Article
			{
				Id = 100,
				Title = "Duplicate Title",
				Content = "Old content",
				Introduction = "Old intro",
				AuthorId = journalist.Id
			};

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new RolesBasedContext(options);
			context.Journalists.Add(journalist);
			context.Articles.Add(existingArticle);
			await context.SaveChangesAsync();

			var controller = new ArticleController(context, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
						new Claim(ClaimTypes.Email, email),
						new Claim(ClaimTypes.Role, "Journalist")
					}))
				}
			};

			var result = await controller.Post(articleDTO);
			var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
			Xunit.Assert.Equal(400, badRequestResult.StatusCode);
		}


		[Fact]
		public async Task Post_ShouldReturnUnauthorized_WhenUserNotFoundByEmail()
		{
			var email = "journalist@example.com";
			var otheremail = "journalist2@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };
			var journalist = new Journalist { Id = 1, ApplicationUserId = "1" };

			var articleDTO = new ArticleDTO
			{
				Title = "Test Article",
				Content = "This is a test content.",
				Introduction = "This is a test introduction."
			};

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new RolesBasedContext(options);
			context.Journalists.Add(journalist);
			await context.SaveChangesAsync();

			var controller = new ArticleController(context, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
						new Claim(ClaimTypes.Email, otheremail),
						new Claim(ClaimTypes.Role, "Journalist")
					}))
				}
			};

			var result = await controller.Post(articleDTO);
			var unauthorizedResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
			Xunit.Assert.Equal(400, unauthorizedResult.StatusCode);
		}










		[Fact]
		public async Task Delete_ShouldRemoveArticle_WhenUserCanDelete()
		{
			var email = "journalist@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };
			var journalist = new Journalist { Id = 1, ApplicationUserId = user.Id, ApplicationUser = user };

			var article = new Article
			{
				Id = 1,
				Title = "Test Article",
				Content = "Some content",
				Introduction = "Intro",
				AuthorId = journalist.Id,
				Author = journalist
			};
			journalist.Articles = new List<Article> { article };

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using var context = new RolesBasedContext(options);
			context.Users.Add(user);
			context.Journalists.Add(journalist);
			context.Articles.Add(article);
			await context.SaveChangesAsync();

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Journalist" });
			mockUserManager.Setup(m => m.Users).Returns(context.Users);

			var controller = new ArticleController(context, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.Role, "Journalist")
				}, "Bearer"))
				}
			};

			var result = controller.Delete(article.Id);
			var okResult = Xunit.Assert.IsType<OkResult>(result);

			var articleInDb = await context.Articles.FindAsync(article.Id);
			Xunit.Assert.Null(articleInDb);
		}

		[Fact]
		public async Task Delete_ShouldReturnNotFound_WhenArticleDoesNotExist()
		{
			var email = "journalist@example.com";
			var user = new ApplicationUser { Id = "1", Email = email };

			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			mockUserManager.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			var options = new DbContextOptionsBuilder<RolesBasedContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new RolesBasedContext(options);

			var controller = new ArticleController(context, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new[]
					{
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.Role, "Journalist")
				}))
				}
			};

			var result = controller.Delete(1);
			var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
			Xunit.Assert.Equal(404, notFoundResult.StatusCode);
		}
	}

}
