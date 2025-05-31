using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MagazinEAPI.Controllers;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedLibrary.DTO_Classes;
using Xunit;
using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Users.Journalists;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using MagazinEAPI.Models.Users.Editors;

namespace MagazinEAPI.Tests.Controllers
{
    public class ArticleControllerTests
    {
        private readonly Mock<RolesBasedContext> _mockContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ArticleController _controller;

        public ArticleControllerTests()
        {
            _mockContext = new Mock<RolesBasedContext>(new DbContextOptions<RolesBasedContext>());

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _controller = new ArticleController(_mockContext.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task Get_ReturnsArticle_WhenExistsAndAuthorized()
        {
            // Arrange
            var testArticle = new Article
            {
                Id = 1,
                Title = "Test Article",
                Comments = new List<Comment>(),
                Tags = new List<Tag>(),
                Photos = new List<Photo>()
            };
            var testUser = new ApplicationUser { Email = "test@example.com" };

            // Create mock DbSet with async support
            var mockArticleDbSet = CreateAsyncMockDbSet(new List<Article> { testArticle }.AsQueryable());
            _mockContext.Setup(x => x.Articles).Returns(mockArticleDbSet.Object);

            // Setup UserManager to return test user
            var mockUserDbSet = CreateAsyncMockDbSet(new List<ApplicationUser> { testUser }.AsQueryable());
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            SetupUserContext("test@example.com");

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                               Mock.Of<IRoleStore<IdentityRole>>(),
                                              null, null, null, null);
            _mockUserManager.Setup(x => x.IsInRoleAsync(testUser, "Admin"));

            // Setup empty journalist and editor sets
            var mockJournalistDbSet = CreateAsyncMockDbSet(new List<Journalist>().AsQueryable());
            _mockContext.Setup(x => x.Journalists).Returns(mockJournalistDbSet.Object);

            var mockEditorDbSet = CreateAsyncMockDbSet(new List<Editor>().AsQueryable());
            _mockContext.Setup(x => x.Editors).Returns(mockEditorDbSet.Object);

            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Admin" });
            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnValue = Xunit.Assert.IsType<ArticleDTO>(okResult.Value);
            Xunit.Assert.Equal(testArticle.Title, returnValue.Title);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            var mockDbSet = CreateAsyncMockDbSet(new List<Article>().AsQueryable());
            _mockContext.Setup(x => x.Articles).Returns(mockDbSet.Object);
            SetupUserContext("test@example.com");

            // Act
            var result = await _controller.Get(1);

            // Assert
            Xunit.Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Put_UpdatesArticle_WhenAuthorized()
        {
            // Arrange
           
            var testUser = new ApplicationUser { Id = "1", Email = "author@example.com" };
            var testJournalist = new Journalist { Id = 1, ApplicationUserId = "1" };
            var testArticle = new Article { Id = 1, Title = "Old Title", Author = testJournalist, AuthorId = testJournalist.Id };
            testJournalist.Articles = new List<Article> { testArticle };

            var mockArticleDbSet = CreateAsyncMockDbSet(new List<Article> { testArticle }.AsQueryable());
            _mockContext.Setup(x => x.Articles).Returns(mockArticleDbSet.Object);

            var mockUserDbSet = CreateAsyncMockDbSet(new List<ApplicationUser> { testUser }.AsQueryable());
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            var mockJournalistDbSet = CreateAsyncMockDbSet(new List<Journalist> { testJournalist }.AsQueryable());
            _mockContext.Setup(x => x.Journalists).Returns(mockJournalistDbSet.Object);

            _mockUserManager.Setup(x => x.GetRolesAsync(testUser));

            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Journalist" });


            SetupUserContext("author@example.com", "Journalist");

            var articleDto = new ArticleDTO { Title = "New Title" };

            // Act
            var result = await _controller.Put(1, articleDto);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
            Xunit.Assert.Equal("New Title", testArticle.Title);
            _mockContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task Delete_RemovesArticle_WhenAuthorized()
        {
            // Arrange
           

            var testUser = new ApplicationUser
            {
                Email = "editor@example.com",
                Id = "editor-2",
            };
            var testArticle = new Article
            {
                Id = 1,
                Title = "Test Article",
                Author = new Journalist
                {
                    Id = 1,
                    ApplicationUser = testUser,
                    ApplicationUserId = testUser.Id,
                },
                AuthorId = 1,
            };
            var testEditor = new Editor
            {
                Id = 1,
                ApplicationUserId = "editor-1",
            };
            testArticle.Author.Articles = new List<Article> { testArticle };

            // Setup Articles DbSet
            var mockArticleDbSet = CreateAsyncMockDbSet(new List<Article> { testArticle }.AsQueryable());
            _mockContext.Setup(x => x.Articles).Returns(mockArticleDbSet.Object);

            // Setup Users DbSet
            var mockUserDbSet = CreateAsyncMockDbSet(new List<ApplicationUser> { testUser }.AsQueryable());
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            _mockContext.Setup(x => x.Users)
                .Returns(CreateAsyncMockDbSet(new List<ApplicationUser> { testUser }.AsQueryable()).Object);

            // Setup Editors DbSet with proper collection initialization
            var mockEditorDbSet = CreateAsyncMockDbSet(new List<Editor> { testEditor }.AsQueryable());
            _mockContext.Setup(x => x.Editors).Returns(mockEditorDbSet.Object);

            // Setup Journalists DbSet
            var mockJournalistDbSet = CreateAsyncMockDbSet(new List<Journalist> { testArticle.Author }.AsQueryable());
            _mockContext.Setup(x => x.Journalists).Returns(mockJournalistDbSet.Object);

            SetupUserContext("editor@example.com", "Editor");

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                               Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Journalist" });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetList_ReturnsArticles_WhenValidRequest()
        {
            // Arrange
            var testArticles = new List<Article>
            {
                new Article { Id = 1, Title = "Article 1", isPublished = true },
                new Article { Id = 2, Title = "Article 2", isPublished = true }
            };

            var mockDbSet = CreateAsyncMockDbSet(testArticles.AsQueryable());
            _mockContext.Setup(x => x.Articles).Returns(mockDbSet.Object);

            var requestDto = new ArticlesRequestDTO { BatchSize = 10, Page = 0 };

            // Act
            var result = await _controller.Get(requestDto);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnValue = Xunit.Assert.IsType<List<ArticleDTO>>(okResult.Value);
            Xunit.Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task Post_CreatesArticle_WhenValidRequest()
        {
            // Arrange
            var testUser = new ApplicationUser { Id = "1", Email = "journalist@example.com" };
            var testJournalist = new Journalist { Id = 1, ApplicationUserId = "1" };

            _mockUserManager.Setup(x => x.FindByEmailAsync("journalist@example.com"))
                .ReturnsAsync(testUser);

            var mockJournalistDbSet = CreateAsyncMockDbSet(new List<Journalist> { testJournalist }.AsQueryable());
            _mockContext.Setup(x => x.Journalists).Returns(mockJournalistDbSet.Object);

            SetupUserContext("journalist@example.com", "Journalist");


            var articleDto = new ArticleDTO { Title = "New Article" };

            _mockContext.Setup(x => x.Articles)
                .Returns(CreateAsyncMockDbSet(new List<Article>().AsQueryable()).Object);
            _mockContext.Setup(x => x.Articles.AddAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Article a, CancellationToken _) => null);

            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Journalist" });


            // Act
            var result = await _controller.Post(articleDto);

            // Assert
            Xunit.Assert.IsType<CreatedResult>(result);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private void SetupUserContext(string email, string role = null)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        private Mock<DbSet<T>> CreateAsyncMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            // Standard synchronous setup
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Async setup
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            return mockSet;
        }

        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_inner.MoveNext());
            }

            public T Current => _inner.Current;
        }

        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(IQueryProvider)
                    .GetMethod(
                        name: nameof(IQueryProvider.Execute),
                        genericParameterCount: 1,
                        types: new[] { typeof(Expression) })
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(this, new[] { expression });

                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult });
            }
        }

        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }
    }
}