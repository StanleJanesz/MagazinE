namespace MagazinEAPI.Models
{

	public class EditorDTO : RoleDTO
	{
		public List<int> ArticlesIds { get; set; }
		public List<int> AllowedTagsIds { get; set; }
	}
	public class Editor: Role
	{
		public int HeadEditorId { get; set; } //has 1 HeadEditor
		public HeadEditor HeadEditor { get; set; } = null!; //has 1 HeadEditor
		public List<Article> Articles { get; set; } = []; //has 0...n articles (0..1 - to - 0..n)

		public List<Tag> AllowedTags { get; set; } = []; 
		public List<TagEditor> AllowedTagEditors { get; set; } = []; //has 1...n allowedTags (many-to-many)

		public List<PublishRequest> PublishRequests { get; set; } = []; // one editor can have many publish request (0/1...n)
	}

}
