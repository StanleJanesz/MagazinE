using MagazinEAPI.Models.Articles;

namespace MagazinEAPI.Models.Users.Readers
{
    public class ToReadArticle
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;
    }
}
