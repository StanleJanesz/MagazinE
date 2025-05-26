using System.Security.Claims;
using MagazinEAPI.Controllers;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Journalists;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedLibrary.DTO_Classes;
using Xunit;
using MagazinEAPI.Models.Users.Editors;
using System.Linq.Expressions;
using System.Collections;
using Microsoft.EntityFrameworkCore.Query;

namespace UnitTests.MagazienEAPI.Controllers
{
    public class JournalistControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RolesBasedContext> _mockContext;
        private readonly JournalistController _controller;

        public JournalistControllerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _mockContext = new Mock<RolesBasedContext>(new DbContextOptions<RolesBasedContext>());

            _controller = new JournalistController(_mockContext.Object, _mockUserManager.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "head.editor@example.com"),
                new Claim(ClaimTypes.Role, "HeadEditor")
            }, "Bearer"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task Get_ReturnsJournalistIds_ForHeadEditor()
        {
            var headEditor = new HeadEditor
            {
                ApplicationUserId = "1",
                JournalistsUnder = new List<Journalist>
                {
                    new Journalist { Id = 1 },
                    new Journalist { Id = 2 }
                }
            };

            var user = new ApplicationUser { Id = "1", Email = "head.editor@example.com" };

            _mockUserManager.Setup(x => x.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable());


            var mockUsersDbSet = MockDbSet(new List<ApplicationUser> { user });
            _mockUserManager.Setup(x => x.Users)
                .Returns(mockUsersDbSet.Object);

            var mockDbSet = MockDbSet(new List<HeadEditor> { headEditor });
            _mockContext.Setup(x => x.HeadEditors)
                .Returns(mockDbSet.Object);

            _mockContext.Setup(x => x.Journalists)
                .Returns(MockDbSet(headEditor.JournalistsUnder).Object);

            var result = await _controller.Get();

            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var journalistIds = Xunit.Assert.IsAssignableFrom<ICollection<int>>(okResult.Value);
            Xunit.Assert.Equal(2, journalistIds.Count);
        }

        [Fact]
        public async Task Get_ReturnsBadRequest_WhenEmailClaimMissing()
        {
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() }
            };

            var result = await _controller.Get();

            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsBadRequest_WhenUserNotFound()
        {
            _mockUserManager.Setup(x => x.Users)
                .Returns(new List<ApplicationUser>().AsQueryable());

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() }
            };



            var result = await _controller.Get();

            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Get_WithId_ReturnsJournalistDto()
        {
            var journalist = new Journalist { Id = 1, ApplicationUser = new ApplicationUser() };
            var mockDbSet = MockDbSet(new List<Journalist> { journalist });
            _mockContext.Setup(x => x.Journalists)
                .Returns(mockDbSet.Object);

            var result = await _controller.Get(1);

            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.IsType<JournalistDTO>(okResult.Value);
        }

        [Fact]
        public async Task Get_WithId_ReturnsNotFound_WhenJournalistMissing()
        {
            var mockDbSet = MockDbSet(new List<Journalist>());
            _mockContext.Setup(x => x.Journalists)
                .Returns(mockDbSet.Object);

            var result = await _controller.Get(1);

            Xunit.Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsJournalistDto_WhenSuccessful()
        {
            var headEditor = new HeadEditor
            {
                Id = 1,
                ApplicationUser = new ApplicationUser { Email = "head.editor@example.com" },
                JournalistsUnder = new List<Journalist>()
            };

            var newUser = new ApplicationUser { Id = "2", Email = "new.journalist@example.com" };

            var mockHeadEditorsDbSet = MockDbSet(new List<HeadEditor> { headEditor });
            _mockContext.Setup(x => x.HeadEditors)
                .Returns(mockHeadEditorsDbSet.Object);

            var mockUsersDbSet = MockDbSet(new List<ApplicationUser> { newUser });
            _mockUserManager.Setup(x => x.Users)
                .Returns(mockUsersDbSet.Object);
            _mockContext.Setup(x => x.Users)
                .Returns(mockUsersDbSet.Object);
            _mockContext.Setup(x => x.Journalists)
                .Returns(MockDbSet(new List<Journalist>()).Object);
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(newUser);

            var result = await _controller.Create("new.journalist@example.com");

            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.IsType<JournalistDTO>(okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenJournalistExists()
        {
            var headEditor = new HeadEditor
            {
                Id = 1,
                ApplicationUser = new ApplicationUser { Email = "head.editor@example.com" },
                JournalistsUnder = new List<Journalist>
                {
                    new Journalist { ApplicationUser = new ApplicationUser { Email = "existing@example.com" } }
                }
            };

            var mockHeadEditorsDbSet = MockDbSet(new List<HeadEditor> { headEditor });
            _mockContext.Setup(x => x.HeadEditors)
                .Returns(mockHeadEditorsDbSet.Object);

            var result = await _controller.Create("existing@example.com");

            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccessful()
        {
            var journalist = new Journalist
            {
                Id = 1,
                HeadEditorId = 1,
                HeadEditor = new HeadEditor
                {
                    Id = 1,
                    ApplicationUser = new ApplicationUser { Email = "head.editor@example.com" }
                }
            };

            var mockDbSet = MockDbSet(new List<Journalist> { journalist });
            _mockContext.Setup(x => x.Journalists)
                .Returns(mockDbSet.Object);

            var result = await _controller.Delete(1);

            Xunit.Assert.IsType<OkObjectResult>(result);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }


        [Fact]
        public async Task GetPersonalInfo_ReturnsUserDto()
        {
            var journalist = new Journalist
            {
                Id = 1,
                ApplicationUser = new ApplicationUser()
            };

            var mockDbSet = MockDbSet(new List<Journalist> { journalist });
            _mockContext.Setup(x => x.Journalists)
                .Returns(mockDbSet.Object);

            var result = await _controller.GetPersonalInfo(1);

            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.IsType<ApplicationUserDTO>(okResult.Value);
        }

        private static Mock<DbSet<T>> MockDbSet<T>(List<T> elements) where T : class
        {
            var queryable = elements.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            dbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

            return dbSet;
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