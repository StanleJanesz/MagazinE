using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
namespace MagazinEAPI.Models.Articles.Comment
{
    public class CommentReport : CommentReportAbstract
    {
        public Comment Comment { get; set; } = null!; // has 1 comment
        public User ReportAuthor { get; set; } = null!; //has 1 report author

        public int? ManagedById { get; set; }
        public Admin? ManagedBy { get; set; } //has 0...1 ManagedBy (Admin)


        public CommentReportDTO ToDTO()
        {
            return new CommentReportDTO()
            { 
                Id = this.Id,
                CommentId = this.CommentId,
                ReportAuthorId = this.ReportAuthorId,
                Reason = this.Reason,
                Date = this.Date,
                State = this.State,
                ManagedById = this.ManagedById ?? -1
            };

        }

		public CommentReportDTO RestrictedToDTO()
		{
			return new CommentReportDTO()
			{
				Id = this.Id,
				CommentId = this.CommentId,
				ReportAuthorId = -1,
				Reason = this.Reason,
				Date = this.Date,
				State = this.State,
				ManagedById = -1
			};

		}
	}

}
