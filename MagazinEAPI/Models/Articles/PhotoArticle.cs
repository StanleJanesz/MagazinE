namespace MagazinEAPI.Models.Articles
{
    public class PhotoArticle
    {
        public int PhotoId { get; set; }
        public int ArticleId { get; set; }
        public Photo Photo { get; set; } = null!;
        public Article Article { get; set; } = null!;
    }
}
