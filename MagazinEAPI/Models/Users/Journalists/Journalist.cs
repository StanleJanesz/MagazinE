namespace MagazinEAPI.Models.Users.Journalists
{
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Articles;
    using MagazinEAPI.Models.Users;
    using MagazinEAPI.Models.Users.Editors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using SharedLibrary.DTO_Classes;

    /// <summary>
    /// Journalist class representing a user with the role of a journalist.
    /// Stored in the database.
    /// </summary>
    public class Journalist : Role
    {
        /// <summary>
        /// Gets or sets the ID of the head editor.
        /// </summary>
        public int HeadEditorId { get; set; } // has 1 HeadEditor

        /// <summary>
        /// Gets or sets the head editor associated with this journalist.
        /// </summary>
        public HeadEditor HeadEditor { get; set; } = null!; // has 1 HeadEditor (1 - to - 1...n)

        /// <summary>
        /// Gets or sets the list of articles written by this journalist.
        /// </summary>
        public List<Article> Articles { get; set; } = []; // has 0...n Articles (1 - to - 0..n)

        /// <summary>
        /// Adapts the Journalist to a DTO.
        /// </summary>
        /// <returns>DTO object of Journalist.</returns>
        public JournalistDTO ToDTO()
        {
            var journalistDTO = new JournalistDTO
            {
                Id = this.Id,
                ArticlesIds = this.Articles.Select(a => a.Id).ToList(),
                PublishRequestsIds = this.Articles.SelectMany(a => a.PublishRequests).Select(pr => pr.Id).ToList(),
                PersonInfoId = this.ApplicationUserId,
            };
            return journalistDTO;
        }
    }
}
