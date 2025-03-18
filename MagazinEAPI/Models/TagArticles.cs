namespace MagazinEAPI.Models
{
	public class TagArticle
	{
		public int TagId { get; set; }
		public int ArticleId { get; set; }
		public Tag Tag { get; set; } = null!;
		public Article Article { get; set; } = null!;
	}
}
