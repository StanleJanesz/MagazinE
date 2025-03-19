namespace MagazinEAPI.Models
{
	public abstract class PublishRequestAbstract
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int ArticleId { get; set; }
		public PublishState PublishState { get; set; }
	}

	public class PublishRequestDTO : PublishRequestAbstract
	{
		public int ReviewerId { get; set; }

	}

	public class PublishRequest : PublishRequestAbstract
	{
		public Article Article { get; set; } = null!; // has 1 Article


		public int? ReviewerId { get; set; } 
		public Editor? Reviewer { get; set; } //has 0...1 Reviewer

	}
}
