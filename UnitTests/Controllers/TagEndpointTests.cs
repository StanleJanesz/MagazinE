using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagazinEAPI.Controllers;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedLibrary.DTO_Classes;
using Xunit;
using MagazinEAPI.Models.Users;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace UnitTests.MagazienEAPI.Controllers
{
    public class TagsControllerTests
    {
        private readonly Mock<RolesBasedContext> _mockContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly TagsController _controller;

        public TagsControllerTests()
        {
            _mockContext = new Mock<RolesBasedContext>(new DbContextOptions<RolesBasedContext>());

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _controller = new TagsController(_mockContext.Object, _mockUserManager.Object);

            // Setup authorized user context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "HeadEditor")
            }, "Bearer"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task Get_ReturnsTag_WhenTagExists()
        {
            // Arrange
            var testTag = new Tag { Id = 1, Name = "Test Tag" };
            _mockContext.Setup(x => x.Tags.FindAsync(1))
                .ReturnsAsync(testTag);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnValue = Xunit.Assert.IsType<TagDTO>(okResult.Value);
            Xunit.Assert.Equal(testTag.Name, returnValue.Name);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(x => x.Tags.FindAsync(1))
                .ReturnsAsync((Tag)null);

            // Act
            var result = await _controller.Get(1);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ReturnsAllTags()
        {
            // Arrange
            var testTags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Tag 1" },
                new Tag { Id = 2, Name = "Tag 2" }
            };

            var mockDbSet = MockDbSet(testTags.AsQueryable());
            _mockContext.Setup(x => x.Tags).Returns(mockDbSet.Object);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnValue = Xunit.Assert.IsType<List<TagDTO>>(okResult.Value);
            Xunit.Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenNoTagsExist()
        {
            // Arrange
            var emptyTags = new List<Tag>();
            var mockDbSet = MockDbSet(emptyTags.AsQueryable());
            _mockContext.Setup(x => x.Tags).Returns(mockDbSet.Object);

            // Act
            var result = await _controller.Get();

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_CreatesNewTag()
        {
            // Arrange
            var newTagDto = new TagDTO { Name = "New Tag" };
            var tags = new List<Tag>();
            var mockDbSet = MockDbSet(tags.AsQueryable());
            _mockContext.Setup(x => x.Tags).Returns(mockDbSet.Object);
            _mockContext.Setup(x => x.Tags.AddAsync(It.IsAny<Tag>(), default))
                .Callback<Tag, CancellationToken>((t, _) => tags.Add(t))
                .ReturnsAsync((EntityEntry<Tag>)null);
            _mockContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Post(newTagDto);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnValue = Xunit.Assert.IsType<TagDTO>(okResult.Value);
            Xunit.Assert.Equal(newTagDto.Name, returnValue.Name);
            Xunit.Assert.Single(tags);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.Post(null);

            // Assert
            Xunit.Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesTag_WhenTagExists()
        {
            // Arrange
            var testTag = new Tag { Id = 1, Name = "Test Tag" };
            _mockContext.Setup(x => x.Tags.FindAsync(1))
                .ReturnsAsync(testTag);
            _mockContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnValue = Xunit.Assert.IsType<TagDTO>(okResult.Value);
            Xunit.Assert.Equal(testTag.Name, returnValue.Name);
            _mockContext.Verify(x => x.Tags.Remove(testTag), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(x => x.Tags.FindAsync(1))
                .ReturnsAsync((Tag)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }

        private Mock<DbSet<T>> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }

    // Helper classes for async support
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