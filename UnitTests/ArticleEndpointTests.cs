using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using SharedLibrary.DTO_Classes;
using SharedLibrary.Base_Classes___Database;
using MagazinEAPI;
using System.Text.Json;
using Moq;
using MagazinEAPI.Contexts;
using MagazinEAPI.Controllers;
using Microsoft.AspNetCore.Identity;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Articles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Users.Journalists;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.Models.Users.Editors;
using Microsoft.AspNetCore.Http;

namespace UnitTests
{
    public class ArticleControllerTests
    {
        [Fact]
        public void GetArticles_ReturnsList_OneElement()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article { Id = 1, Title = "Test", isPublished = true }
            };

            var controller = Arrange(articles);
            var requestDto = new ArticlesRequestDTO
            {
                BatchSize = 10,
                Page = 0
            };

            // Act
            var result = controller.Get(requestDto);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedArticles = Xunit.Assert.IsAssignableFrom<IEnumerable<ArticleDTO>>(okResult.Value);
            Xunit.Assert.Single(returnedArticles);
        }
        [Fact]
        public void GetArticles_ReturnsList_ManyElements()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article { Id = 1, Title = "Test", isPublished = true },
                new Article { Id = 2, Title = "Test", isPublished = false },
                new Article { Id = 3, Title = "Test", isPublished = true },
                new Article { Id = 4, Title = "Test", isPublished = true },
                new Article { Id = 5, Title = "Test", isPublished = true },
                new Article { Id = 6, Title = "Test", isPublished = true },
                new Article { Id = 7, Title = "Test", isPublished = true },
                new Article { Id = 8, Title = "Test", isPublished = true },
                new Article { Id = 9, Title = "Test", isPublished = true },
                new Article { Id = 10, Title = "Test", isPublished = true },
                new Article { Id = 11, Title = "Test", isPublished = true },
            };

            var controller = Arrange(articles);
            var requestDto = new ArticlesRequestDTO
            {
                BatchSize = 10,
                Page = 0
            };

            // Act
            var result = controller.Get(requestDto);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedArticles = Xunit.Assert.IsAssignableFrom<IEnumerable<ArticleDTO>>(okResult.Value);

            // Check that exactly 10 articles are returned
            Xunit.Assert.Equal(10, returnedArticles.Count());

            // Check that all returned articles are published
            Xunit.Assert.All(returnedArticles, article => Xunit.Assert.True(article.isPublished && article.Id != 2));
        }

        private ArticleController Arrange(List<Article> articles)
        {
            var mockArticleSet = MockDbSetHelper.CreateMockDbSet(articles);

            var mockContext = new Mock<RolesBasedContext>(new DbContextOptions<RolesBasedContext>());
            mockContext.Setup(c => c.Articles).Returns(mockArticleSet.Object);

            // Mock UserManager<ApplicationUser>
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new ArticleController(mockContext.Object, mockUserManager.Object);

            // Simulate HTTP Context if needed for authentication/authorization
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            return controller;
        }
    }
}