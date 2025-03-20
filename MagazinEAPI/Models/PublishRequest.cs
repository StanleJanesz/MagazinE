using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models
{
	public class PublishRequest : PublishRequestAbstract
	{
		public Article Article { get; set; } = null!; // has 1 Article -> ArticleId in abstractClass


		public int? ReviewerId { get; set; } 
		public Editor? Reviewer { get; set; } //has 0...1 Reviewer

	}
}
