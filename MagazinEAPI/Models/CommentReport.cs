namespace MagazinEAPI.Models
{
	public abstract class CommentReportAbstract
	{
		public int Id { get; set; }
		public int CommentId { get; set; }
		public int ReportAuthorId { get; set; }
		public string Reason { get; set; }
		public DateTime Date { get; set; }
		public bool Result { get; set; }
	}

	public class CommentReportDTO : CommentReportAbstract
	{
		public int ManagedById { get; set; }
	}
	public class CommentReport: CommentReportAbstract
	{
		public Comment Comment { get; set; } = null!; // has 1 comment
		public User ReportAuthor { get; set; } = null!; //has 1 report author

		public int? ManagedById { get; set; }
		public Admin? ManagedBy { get; set; } //has 0...1 ManagedBy (Admin)
	}

}
