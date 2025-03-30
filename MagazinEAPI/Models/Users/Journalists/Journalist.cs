using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Articles;
namespace MagazinEAPI.Models.Users.Journalists
{
    public class Journalist : Role
    {
        public int HeadEditorId { get; set; } //has 1 HeadEditor
        public HeadEditor HeadEditor { get; set; } = null!; //has 1 HeadEditor (1 - to - 1...n)
        public List<Article> Articles { get; set; } = []; //has 0...n Articles (1 - to - 0..n)

        // we get PublishRequests from articles !!!
    }
}
