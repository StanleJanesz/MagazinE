namespace MagazinEAPI.Models
{

	public abstract class CommentAbstract
	{
		public int Id { get; set; }
		public int AuthorId { get; set; }
		public string Content { get; set; }
		public DateTime Date { get; set; }
	}

	public class CommentDTO : CommentAbstract
	{
		public int ParentId { get; set; }
		public List<int> ChildrenIds { get; set; }

		public int LikesCount { get; set; }
		public int DislikesCount { get; set; }
	}
	public class Comment : CommentAbstract
	{

		//public bool IsDeleted { get; set; }
		public int? DeletedById { get; set; }
		public Admin? DeletedBy { get; set; } //has 0..1 deletedBy (Admin) (0..1 to 0...n)

		public int ArticleId { get; set; }
		public Article Article { get; set; } = null!; //has 1 article (1 to many)

		public User Author { get; set; } = null!;//has 1 author (1 to many)

		public List<User> LikeUsers { get; set; } = [];
		public List<Like> Likes { get; set; } = []; //represents many-to-many User-Comment

		public List<User> DislikeUsers { get; set; } = [];
		public List<Dislike> Dislikes { get; set; } = []; //represents many-to-many User-Comment

		public int? ParentId { get; set; } 
		public Comment? Parent { get; set; } //has 0..1 parent 
		public List<Comment> Children { get; set; } = []; //has 0..n children 

		public List<CommentReport> Reports { get; set; } = []; //one to many
	}


}
