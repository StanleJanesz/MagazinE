namespace MagazinEAPI.Models
{

	public class UserDTO : RoleDTO
	{
		public bool SubscriptionState { get; set; }
		public List <int> FavouriteArticlesIds { get; set; }
		public List<int> ArticlesToReadIds { get; set; }
		public List<int> FavouriteTagsIds { get; set; }
		public List<int> CommentsIds { get; set; }
		public List<int> LikedCommentsIds { get; set; }
		public List<int> UnlikedCommentsIds { get; set; }
	}

	public class User : Role
	{
		public List<Subscription> Subscriptions { get; set; } = []; //represents one-to-many User-Subscription
		public List<Ban> Bans { get; set; } = []; //represents one-to-many User-Ban
		public List<Article> ArticleFavoriteArticles { get; set; } = [];
		public List<FavoriteArticle> FavoriteArticles { get; set; } = []; //represents many-to-many User-Article
		public List<Article> ArticleToReadArticles { get; set; } = [];
		public List<ToReadArticle> ToReadArticles { get; set; } = [];//represents many-to-many User-Article
		public List<Tag> FavouriteTags { get; set; } = [];
		public List<TagUser> FavouriteTagUsers { get; set; } = []; //represents many-to-many User-Tag
		public List<Comment> Comments { get; set; } = []; // 1 to 0..n 
		public List<CommentReport> ReportedComments { get; set; } = []; //one to many
		public List<Comment> LikeComments { get; set; } = []; 
		public List<Like> Likes { get; set; } = []; //represents many-to-many User-Comment
		public List<Comment> DislikeComments { get; set; } = [];
		public List<Dislike> Dislikes { get; set; } = []; //represents many-to-many User-Comment
	}
}
