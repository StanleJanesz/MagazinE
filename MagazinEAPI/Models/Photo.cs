namespace MagazinEAPI.Models
{
	public class Photo
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public int ArticleId { get; set; } //has 1 article
		public Article Article { get; set; } = null!;  //has 1 article
	}
}
