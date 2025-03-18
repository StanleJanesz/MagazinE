namespace MagazinEAPI.Models
{

	public abstract class ArticleAbstract
	{
		public int Id { get; set; }
		public bool isPremium { get; set; }
		public bool isPublished { get; set; }
		public string Title { get; set; }
		public int AuthorId { get; set; }
		public string Content { get; set; }
		public DateTime TimeOfPublication { get; set; }
	}

	public class ArticleDTO : ArticleAbstract
	{
		public List<int> CommentsIds { get; set; }
		public List<string> Photos { get; set; }
		public List<int> TagsIds { get; set; }

	}

	public class Article : ArticleAbstract
	{
		public Journalist Author { get; set; } = null!; //has 1 Author (1 - to - 0..n)
		public List<Comment> Comments { get; set; } = []; //has 0...n Comments (1 - to 0..n)
		public int? ReviewerId { get; set; } //has 0...1 reviewers
		public Editor? Reviewer { get; set; } //has 0...1 reviewers (0..1 - to - 0..n)


		public List<Photo> Photos { get; set; } = []; //has 1...n Photos (1 - to - 1..n)
		public List<PublishRequest> PublishRequests { get; set; } = []; // one article can have many publish request ???


		public List<Tag> Tags { get; } = [];
		public List<TagArticle> TagArticles { get; set; } = []; //has 1...n Tags (many-to-many)


		public List<User> UserFavoriteArticles { get; set; } = [];
		public List<FavoriteArticle> FavoriteArticles { get; set; } = []; //represents many-to-many User-Article


		public List<User> UserToReadArticles { get; set; } = [];
		public List<ToReadArticle> ToReadArticles { get; set; } = [];//represents many-to-many User-Article

	}


}
