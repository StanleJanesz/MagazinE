using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Requests;

namespace MagazinEAPI.Models.Users.Admins
{
    public class Admin : Role
    {
        public List<Ban> Bans { get; set; } = []; //one-to-many 

        public List<Comment> DeletedComments { get; set; } = []; //Admin from Comments (Comment has 0...1 Admin (if comment deleated))
        public List<CommentReport> ManagedCommentReports { get; set; } = []; //one or zero to many (0-1...n)

        public List<UnbanRequest> SolvedUnbanRequests { get; set; } = []; //one to many
        // UnbanRequests - Admin from Bans (Ban has one Admin) and Ban from UnbanRequest (UnbanRequest has 1 Ban)
    }




}
