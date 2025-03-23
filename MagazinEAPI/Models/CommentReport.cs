using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models
{
	public class CommentReport: CommentReportAbstract
	{
		public Comment Comment { get; set; } = null!; // has 1 comment
		public User ReportAuthor { get; set; } = null!; //has 1 report author

		public int? ManagedById { get; set; }
		public Admin? ManagedBy { get; set; } //has 0...1 ManagedBy (Admin)
	}

}
