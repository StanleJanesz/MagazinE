namespace MagazinEAPI.Models
{

	public class AdminDTO : RoleDTO
	{
		public List<int> BannedUsersIds { get; set; }
		public List<int> UnbanRequestsIds { get; set; }
		public List<int> DeletedCommentsIds { get; set; }
	}

	public class Admin : Role
	{
		public List<Ban> Bans { get; set; } = []; //one-to-many 

		public List<Comment> DeletedComments { get; set; } = []; //Admin from Comments (Comment has 0...1 Admin (if comment deleated))
		public List<CommentReport> ManagedCommentReports { get; set; } = []; //one or zero to many (0-1...n)
		
		// UnbanRequests - Admin from Bans (Ban has one Admin) and Ban from UnbanRequest (UnbanRequest has 1 Ban)
	}




}
