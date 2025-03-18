namespace MagazinEAPI.Models
{

	public class JournalistDTO : RoleDTO
	{
		public List<int> ArticlesIds { get; set; }
		public List<int> PublishRequestsIds { get; set; }
	}

	public class Journalist : Role
	{
		public int HeadEditorId { get; set; } //has 1 HeadEditor
		public HeadEditor HeadEditor { get; set; } = null!; //has 1 HeadEditor (1 - to - 1...n)
		public List<Article> Articles { get; set; } = []; //has 0...n Articles (1 - to - 0..n)

		// we get PublishRequests from articles !!!
	}
}
