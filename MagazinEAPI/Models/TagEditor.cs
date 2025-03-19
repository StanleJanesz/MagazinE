namespace MagazinEAPI.Models
{
	public class TagEditor
	{
		public int TagId { get; set; }
		public int EditorId { get; set; }
		public Tag Tag { get; set; } = null!;
		public Editor Editor { get; set; } = null!;
	}
}
