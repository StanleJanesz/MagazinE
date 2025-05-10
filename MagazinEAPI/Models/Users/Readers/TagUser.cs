using MagazinEAPI.Models.Articles;

namespace MagazinEAPI.Models.Users.Readers
{
    public class TagUser
    {
        public int TagId { get; set; }
        public int UserId { get; set; }
        public Tag Tag { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
