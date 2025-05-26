using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
namespace MagazinEAPI.Models.Articles
{
    public class Tag : TagAbstract, DTOable<TagDTO>
    {
        public List<User> Users { get; set; } = [];

        public List<TagUser> TagUsers { get; set; } = []; //has 1...n users (many-to-many)

        public List<Editor> Editors { get; set; } = [];

        public List<TagEditor> TagEditors { get; set; } = []; //has 1...n editors (many-to-many)

        public List<Article> Articles { get; } = [];

        public List<TagArticle> TagArticles { get; set; } = []; //has 1...n articles (many-to-many)


        /// <summary>
        /// returns a DTO object of the current Tag object.
        /// </summary>
        /// <returns>DTO object.</returns>
        public TagDTO ToDTO()
        {
            return new TagDTO
            {
                Id = this.Id,
                Name = this.Name,
            };
        }
    }
}
