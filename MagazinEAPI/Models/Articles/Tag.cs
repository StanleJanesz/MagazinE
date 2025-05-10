using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models.Articles
{
    public class Tag : TagAbstract
    {
        public List<User> Users { get; set; } = [];
        public List<TagUser> TagUsers { get; set; } = []; //has 1...n users (many-to-many)

        public List<Editor> Editors { get; set; } = [];
        public List<TagEditor> TagEditors { get; set; } = []; //has 1...n editors (many-to-many)

        public List<Article> Articles { get; } = [];
        public List<TagArticle> TagArticles { get; set; } = []; //has 1...n articles (many-to-many)

    }
}
