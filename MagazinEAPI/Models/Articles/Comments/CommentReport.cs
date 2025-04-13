namespace MagazinEAPI.Models.Articles.Comment
{
    using MagazinEAPI.Models.Users.Admins;
    using MagazinEAPI.Models.Users.Readers;
    using SharedLibrary.Base_Classes___Database;

    /// <summary>
    /// Represents a report on a comment.
    /// Stored in the database.
    /// </summary>
    public class CommentReport : CommentReportAbstract
    {
        /// <summary>
        /// Gets or sets related comment.
        /// </summary>
        public Comment Comment { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user who reported the comment.
        /// </summary>
        public User ReportAuthor { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the admin who managed the report.
        /// </summary>
        public int? ManagedById { get; set; }

        /// <summary>
        /// Gets or sets the admin who managed the report.
        /// </summary>
        public Admin? ManagedBy { get; set; }
    }

}
