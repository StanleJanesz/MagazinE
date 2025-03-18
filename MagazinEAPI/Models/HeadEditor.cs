namespace MagazinEAPI.Models
{

	public class HeadEditorDTO : RoleDTO
	{
		public List<int> ArticlesIds { get; set; }
		public List<int> EditorsUnderIds { get; set; }
		public List<int> JournalistsUnderIds { get; set; }
	}

	public class HeadEditor : Role
	{
		//HeadEditor get articles from their journalists
		public List<Editor> EditorsUnder { get; set; } = []; //has 0...n Editors
		public List<Journalist> JournalistsUnder { get; set; } = []; //has 0...n Journalist
	}

}
