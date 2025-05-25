using Xunit;
using Moq;
using MagazinEAPI.Controllers;
using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.DTO_Classes;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Journalists;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Requests;
using Newtonsoft.Json.Linq;
using MagazinEAPI.Contexts.Configurations.ArticleConfigurations;

namespace UnitTests;

public class CommentsControllerTests
{
	private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
	private readonly RolesBasedContext _context;  // tutaj użyj np. InMemory DbContext
	private readonly CommentsController _controller;
	private ApplicationUser userEntity;
	private User userInfo;
	private Journalist journalist;
	private Editor editor;
	private Admin admin;
	private HeadEditor headEditor;
	private Article article;
	private Comment comment;


	public CommentsControllerTests()
	{
		// Setup in-memory db context, e.g.:
		var options = new DbContextOptionsBuilder<RolesBasedContext>()
			.UseInMemoryDatabase(databaseName: "TestDb")
			.Options;
		_context = new RolesBasedContext(options);

		var store = new Mock<IUserStore<ApplicationUser>>();

		_mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

		var roles = new List<string> { "Reader", "Admin", "Journalist", "Editor", "HeadEditor" };

		_mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>()))
			.ReturnsAsync(roles);

		_controller = new CommentsController(_context, _mockUserManager.Object);

		// Ustawienie User w controllerze z fake email claim
		var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, "test@example.com"),
			new Claim(ClaimTypes.Role, "Reader"),
		}, "mock"));

		_controller.ControllerContext = new ControllerContext()
		{
			HttpContext = new DefaultHttpContext() { User = user }
		};


		SeedData();
	}

	private void SeedData()
	{
		userEntity = new ApplicationUser {  Email = "test@example.com" };
		userInfo = new User { ApplicationUser = userEntity };


		headEditor = new HeadEditor() { ApplicationUser = userEntity };
	    journalist = new Journalist() { ApplicationUser = userEntity, HeadEditor = headEditor };
		editor = new Editor() { ApplicationUser = userEntity, HeadEditor = headEditor };
		admin = new Admin() { ApplicationUser = userEntity };

		userEntity.User = userInfo;
		userEntity.HeadEditor = headEditor;
		userEntity.Journalist = journalist;
		userEntity.Editor = editor;
		userEntity.Admin = admin;

		// Tworzymy artykuł
		article = new Article
		{
			isPublished = true,
			isPremium = false,
			//AuthorId = journalist.Id,
			Author = journalist,
			//ReviewerId = editor.Id,
			Reviewer = editor,
			Content = "some article",
			Title = "some title",
			Introduction = "some Introduction",
			Comments = new List<Comment>(),
			Tags = new List<Tag>(),
			Photos = new List<Photo>(),
			PublishRequests = new List<PublishRequest>(),
			UserFavoriteArticles = new List<User>(),
			UserToReadArticles = new List<User>()
		};

		// Tworzymy komentarz przypisany do artykułu i autora
		comment = new Comment
		{
			Content = "Test comment",
			//ArticleId = article.Id,
			Article = article,
			//AuthorId = userInfo.Id,
			Author = userInfo,
			IsDeleted = false,
			LikeUsers = new List<User>(),
			DislikeUsers = new List<User>(),
			Children = new List<Comment>(),
			Reports = new List<CommentReport>()
		};

		// Dodajemy komentarz do listy komentarzy artykułu i użytkownika
		article.Comments.Add(comment);
		userInfo.Comments.Add(comment);

		// Dodajemy encje do kontekstu
		_context.Readers.Add(userInfo);
		_context.Users.Add(userEntity);
		_context.HeadEditors.Add(headEditor);
		_context.Journalists.Add(journalist);
		_context.Editors.Add(editor);
		_context.Admins.Add(admin);
		_context.Articles.Add(article);
		_context.Comments.Add(comment);

		// Zapisujemy zmiany
		_context.SaveChanges();

		// Mockowanie UserManager - IQueryable<ApplicationUser>
		//var userList = new List<ApplicationUser> { userEntity }.AsQueryable();
		//_mockUserManager.Setup(um => um.Users).Returns(userList);

		// Mockowanie UserManager - IQueryable<ApplicationUser>
		var userList = _context.Users.ToList().AsQueryable();
		_mockUserManager.Setup(um => um.Users).Returns(userList);
	}

	[Fact]
	public void Get_ExistingComment_ReturnsOk()
	{
		var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, "test@example.com"),
			new Claim(ClaimTypes.Role, "Reader"),
		}, "mock"));


		var result = _controller.Get(comment.Id);

		var okResult = Xunit.Assert.IsType<OkObjectResult>(result);

		var dto = Xunit.Assert.IsType<CommentDTO>(okResult.Value);
		Xunit.Assert.Equal(comment.Id, dto.Id);
	}

	[Fact]
	public void Get_NonExistingComment_ReturnsNotFound()
	{
		var result = _controller.Get(comment.Id * 200);

		Xunit.Assert.IsType<NotFoundObjectResult>(result);
	}

	[Fact]
	public void Post_ValidComment_ReturnsOk()
	{
		var readerUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, "test@example.com"),
			new Claim(ClaimTypes.Role, "Reader"),
		}, "mock"));

		_controller.ControllerContext.HttpContext.User = readerUser;


		var newCommentDto = new CommentDTO
		{
			ArticleId = article.Id,
			Content = "New comment content"
		};

		var result = _controller.Post(newCommentDto);

		var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
		var dto = Xunit.Assert.IsType<CommentDTO>(okResult.Value);
		Xunit.Assert.Equal(newCommentDto.Content, dto.Content);
	}

	[Fact]
	public void Post_ArticleNotFound_ReturnsNotFound()
	{
		var newCommentDto = new CommentDTO
		{
			ArticleId = 999,  // nie istnieje
			Content = "New comment content"
		};

		var readerUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, "test@example.com"),
			new Claim(ClaimTypes.Role, "Reader"),
		}, "mock"));

		_controller.ControllerContext.HttpContext.User = readerUser;

		var result = _controller.Post(newCommentDto);

		var xx = Xunit.Assert.IsType<NotFoundObjectResult>(result); //NitFound

	}

	[Fact]
	public void Delete_ExistingComment_AsAdmin_ReturnsOk()
	{
		var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, "test@example.com"),
			new Claim(ClaimTypes.Role, "Admin"),
		}, "mock"));

		_controller.ControllerContext.HttpContext.User = adminUser;


		var comment2 = new Comment
		{
			Content = "Test comment 2",
			Article = article,
			Author = userInfo,
			IsDeleted = false,
			LikeUsers = new List<User>(),
			DislikeUsers = new List<User>(),
			Children = new List<Comment>(),
			Reports = new List<CommentReport>()
		};

		_context.Add(comment2);
		_context.SaveChanges();


		var result = _controller.Delete(comment2.Id);

		Xunit.Assert.IsType<OkResult>(result);

		
	}

	[Fact]
	public void Delete_NonExistingComment_ReturnsNotFound()
	{
		var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, "test@example.com"),
			new Claim(ClaimTypes.Role, "Admin"),
		}, "mock"));

		_controller.ControllerContext.HttpContext.User = adminUser;

		var result = _controller.Delete(comment.Id * 20);

		Xunit.Assert.IsType<NotFoundObjectResult>(result);
	}

	[Fact]
	public void Like_AlreadyLiked_ReturnsBadRequest()
	{
		var comment = _context.Comments.Include(c => c.LikeUsers).First(c => c.Id == 1);
		var user = _context.Readers.First(u => u.Id == 1);
		comment.LikeUsers.Add(user);
		_context.SaveChanges();

		var result = _controller.Like(1);

		var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);
		Xunit.Assert.Contains("already liked", badRequest.Value.ToString());
	}




	[Fact]
	public void Get_CommentWithoutEmailClaim_ReturnsBadRequest()
	{
		_controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

		var result = _controller.Get(comment.Id);

		var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);
		Xunit.Assert.Equal("Email not found", badRequest.Value);
	}

	[Fact]
	public async void Delete_CommentWithoutAdminRole_ReturnsUnauthorized()
	{
		var otherUser = new ApplicationUser { Email = "other@example.com" };
		_context.Add(otherUser);
		await _context.SaveChangesAsync();


		var reader = new User();
		reader.ApplicationUserId = otherUser.Id;
		otherUser.User = reader;
		reader.ApplicationUser = otherUser;

		_context.Add(reader);
		_context.Update(otherUser);
		await _context.SaveChangesAsync();

		var userList = _context.Users.ToList().AsQueryable();
		_mockUserManager.Setup(um => um.Users).Returns(userList);


		var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, otherUser.Email),
			new Claim(ClaimTypes.Role, "Reader"),
		}, "mock"));

		_controller.ControllerContext.HttpContext.User = user;

		var result = _controller.Delete(comment.Id);

		_context.Remove(otherUser);
		_context.Remove(reader);
		await _context.SaveChangesAsync();

		userList = _context.Users.ToList().AsQueryable();
		_mockUserManager.Setup(um => um.Users).Returns(userList);

		var unauthorized = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
		Xunit.Assert.Equal("User is not an admin", unauthorized.Value);
	}

	[Fact]
	public async void Put_EditCommentByAnotherUser_ReturnsUnauthorized()
	{
		var otherUser = new ApplicationUser { Email = "other@example.com" };
		_context.Add(otherUser);
		await _context.SaveChangesAsync();


		var reader = new User();
		reader.ApplicationUserId = otherUser.Id;
		otherUser.User = reader;
		reader.ApplicationUser = otherUser;

		_context.Add(reader);
		_context.Update(otherUser);
		await _context.SaveChangesAsync();

		var userList = _context.Users.ToList().AsQueryable();
		_mockUserManager.Setup(um => um.Users).Returns(userList);


		var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Email, otherUser.Email),
			new Claim(ClaimTypes.Role, "Reader"),
		}, "mock"));

		_controller.ControllerContext.HttpContext.User = user;

		var commentDto = new CommentDTO { Content = "Updated content", Id = 1 };

		var result = _controller.Put(1, commentDto);

		_context.Remove(otherUser);
		_context.Remove(reader);
		await _context.SaveChangesAsync();

		userList = _context.Users.ToList().AsQueryable();
		_mockUserManager.Setup(um => um.Users).Returns(userList);


		var unauthorized = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
		Xunit.Assert.Equal("User can only edit his comments", unauthorized.Value);
	}

	

	[Fact]
	public void Get_CommentFromNotPublishedArticle_ReturnsUnauthorized()
	{
		var article2 = new Article 
		{ 
			isPublished = false, 
			Author = journalist, 
			AuthorId = journalist.Id, 
			isPremium = false,
			Title = "new title",
			Introduction = "some text",
			Content = "some text"

		};

		_context.Articles.Add(article2);
		_context.SaveChanges();


		var comment2 = new Comment
		{
			Id = 2,
			Article = article2,
			ArticleId = article2.Id,
			Author = userEntity.User,
			AuthorId = userEntity.User.Id,
			IsDeleted = false,
			Content = "Comment on unpublished article"
		};

		_context.Comments.Add(comment2);
		_context.SaveChanges();


		var result = _controller.Get(comment2.Id);

		_context.Remove(article2);
		_context.Remove(comment2);
		_context.SaveChanges();

		var unauthorized = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
		Xunit.Assert.Equal("Comment cannot be viewed because article is not published", unauthorized.Value);
	}
}

