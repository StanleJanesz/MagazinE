using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models.Articles.Comment
{
    public class Comment : CommentAbstract
    {

        public bool IsDeleted { get; set; } // means is not visible to users
        public int? DeletedById { get; set; }
        public Admin? DeletedBy { get; set; } //has 0..1 deletedBy (Admin) (0..1 to 0...n)

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!; //has 1 article (1 to many)

        public User Author { get; set; } = null!;//has 1 author (1 to many) --> Id in abstract class

        public List<User> LikeUsers { get; set; } = [];
        public List<Like> Likes { get; set; } = []; //represents many-to-many User-Comment

        public List<User> DislikeUsers { get; set; } = [];
        public List<Dislike> Dislikes { get; set; } = []; //represents many-to-many User-Comment

        public int? ParentId { get; set; }
        public Comment? Parent { get; set; } //has 0..1 parent 
        public List<Comment> Children { get; set; } = []; //has 0..n children 

        public List<CommentReport> Reports { get; set; } = []; //one to many
    }


}
