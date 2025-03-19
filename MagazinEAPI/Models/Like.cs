namespace MagazinEAPI.Models
{
	public class Like
	{
		public int UserId { get; set; }
		public int CommentId { get; set; }

		public User User { get; set; } = null!;
		public Comment Comment { get; set; } = null!;
	}
}
