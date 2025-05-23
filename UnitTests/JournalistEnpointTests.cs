//using System.Security.Claims;
//using MagazinEAPI.Controllers;
//using MagazinEAPI.Contexts;
//using MagazinEAPI.Models.Users;
//using MagazinEAPI.Models.Users.Journalists;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using SharedLibrary.DTO_Classes;
//using Xunit;
//using MagazinEAPI.Models.Users.Editors;

//namespace MagazinEAPI.Tests.Controllers
//{
//    public class JournalistControllerTests
//    {
//        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
//        private readonly Mock<RolesBasedContext> _mockContext;
//        private readonly JournalistController _controller;

//        public JournalistControllerTests()
//        {
//            var store = new Mock<IUserStore<ApplicationUser>>();
//            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
//                store.Object, null, null, null, null, null, null, null, null);

//            _mockContext = new Mock<RolesBasedContext>(new DbContextOptions<RolesBasedContext>());

//            _controller = new JournalistController(_mockContext.Object, _mockUserManager.Object);

//            // Setup controller context with user claims
//            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
//            {
//                new Claim(ClaimTypes.Email, "head.editor@example.com"),
//                new Claim(ClaimTypes.Role, "HeadEditor")
//            }, "Bearer"));

//            _controller.ControllerContext = new ControllerContext()
//            {
//                HttpContext = new DefaultHttpContext() { User = user }
//            };
//        }

//        [Fact]
//        public async Task Get_ReturnsJournalistIds_ForHeadEditor()
//        {
//            // Arrange
//            var headEditor = new HeadEditor
//            {
//                ApplicationUserId = "1",
//                JournalistsUnder = new List<Journalist>
//                {
//                    new Journalist { Id = 1 },
//                    new Journalist { Id = 2 }
//                }
//            };

//            var user = new ApplicationUser { Id = "1", Email = "head.editor@example.com" };

//            _mockUserManager.Setup(x => x.Users)
//                .Returns(new List<ApplicationUser> { user }.AsQueryable());

//            _mockContext.Setup(x => x.HeadEditors)
//                .Returns(MockDbSet(new List<HeadEditor> { headEditor }));

//            // Act
//            var result = await _controller.Get();

//            // Assert
//            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
//            var journalistIds = Xunit.Assert.IsAssignableFrom<ICollection<int>>(okResult.Value);
//            Xunit.Assert.Equal(2, journalistIds.Count);
//        }

//        [Fact]
//        public async Task Get_ReturnsBadRequest_WhenEmailClaimMissing()
//        {
//            // Arrange
//            _controller.ControllerContext = new ControllerContext()
//            {
//                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() }
//            };

//            // Act
//            var result = await _controller.Get();

//            // Assert
//            Xunit.Assert.IsType<BadRequestObjectResult>(result);
//        }

//        [Fact]
//        public async Task Get_ReturnsBadRequest_WhenUserNotFound()
//        {
//            // Arrange
//            _mockUserManager.Setup(x => x.Users)
//                .Returns(new List<ApplicationUser>().AsQueryable());

//            // Act
//            var result = await _controller.Get();
//            var a = 2 + 2;
//            // Assert
//            Xunit.Assert.IsType<BadRequestObjectResult>(result);
//        }

//        [Fact]
//        public async Task Get_WithId_ReturnsJournalistDto()
//        {
//            // Arrange
//            var journalist = new Journalist { Id = 1, ApplicationUser = new ApplicationUser() };
//            _mockContext.Setup(x => x.Journalists)
//                .Returns(MockDbSet(new List<Journalist> { journalist }));

//            // Act
//            var result = await _controller.Get(1);

//            // Assert
//            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
//            Xunit.Assert.IsType<JournalistDTO>(okResult.Value);
//        }

//        [Fact]
//        public async Task Get_WithId_ReturnsNotFound_WhenJournalistMissing()
//        {
//            // Arrange
//            _mockContext.Setup(x => x.Journalists)
//                .Returns(MockDbSet(new List<Journalist>()));

//            // Act
//            var result = await _controller.Get(1);

//            // Assert
//            Xunit.Assert.IsType<NotFoundObjectResult>(result);
//        }

//        [Fact]
//        public async Task Create_ReturnsJournalistDto_WhenSuccessful()
//        {
//            // Arrange
//            var headEditor = new HeadEditor
//            {
//                Id = 1,
//                ApplicationUser = new ApplicationUser { Email = "head.editor@example.com" },
//                JournalistsUnder = new List<Journalist>()
//            };

//            var newUser = new ApplicationUser { Id = "2", Email = "new.journalist@example.com" };

//            _mockContext.Setup(x => x.HeadEditors)
//                .Returns(MockDbSet(new List<HeadEditor> { headEditor }));

//            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
//                .ReturnsAsync(newUser);

//            // Act
//            var result = await _controller.Create("new.journalist@example.com");

//            // Assert
//            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
//            Xunit.Assert.IsType<JournalistDTO>(okResult.Value);
//        }

//        [Fact]
//        public async Task Create_ReturnsBadRequest_WhenJournalistExists()
//        {
//            // Arrange
//            var headEditor = new HeadEditor
//            {
//                Id = 1,
//                ApplicationUser = new ApplicationUser { Email = "head.editor@example.com" },
//                JournalistsUnder = new List<Journalist>
//                {
//                    new Journalist { ApplicationUser = new ApplicationUser { Email = "existing@example.com" } }
//                }
//            };

//            _mockContext.Setup(x => x.HeadEditors)
//                .Returns(MockDbSet(new List<HeadEditor> { headEditor }));

//            // Act
//            var result = await _controller.Create("existing@example.com");

//            // Assert
//            Xunit.Assert.IsType<BadRequestObjectResult>(result);
//        }

//        [Fact]
//        public async Task Delete_ReturnsOk_WhenSuccessful()
//        {
//            // Arrange
//            var journalist = new Journalist
//            {
//                Id = 1,
//                HeadEditorId = 1,
//                HeadEditor = new HeadEditor
//                {
//                    Id = 1,
//                    ApplicationUser = new ApplicationUser { Email = "head.editor@example.com" }
//                }
//            };

//            _mockContext.Setup(x => x.Journalists)
//                .Returns(MockDbSet(new List<Journalist> { journalist }));

//            // Act
//            var result = await _controller.Delete(1);

//            // Assert
//            Xunit.Assert.IsType<OkObjectResult>(result);
//            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
//        }

//        [Fact]
//        public async Task Delete_ReturnsUnauthorized_WhenNotHeadEditor()
//        {
//            // Arrange
//            var journalist = new Journalist
//            {
//                Id = 1,
//                HeadEditorId = 2, // Different head editor
//                HeadEditor = new HeadEditor { Id = 2 }
//            };

//            _mockContext.Setup(x => x.Journalists)
//                .Returns(MockDbSet(new List<Journalist> { journalist }));

//            // Act
//            var result = await _controller.Delete(1);

//            // Assert
//            Xunit.Assert.IsType<UnauthorizedResult>(result);
//        }

//        [Fact]
//        public async Task GetPersonalInfo_ReturnsUserDto()
//        {
//            // Arrange
//            var journalist = new Journalist
//            {
//                Id = 1,
//                ApplicationUser = new ApplicationUser()
//            };

//            _mockContext.Setup(x => x.Journalists)
//                .Returns(MockDbSet(new List<Journalist> { journalist }));

//            // Act
//            var result = await _controller.GetPersonalInfo(1);

//            // Assert
//            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
//            Xunit.Assert.IsType<ApplicationUserDTO>(okResult.Value);
//        }

//        private static DbSet<T> MockDbSet<T>(List<T> elements) where T : class
//        {
//            var queryable = elements.AsQueryable();
//            var dbSet = new Mock<DbSet<T>>();
//            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
//            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
//            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
//            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
//            return dbSet.Object;
//        }
//    }
//}