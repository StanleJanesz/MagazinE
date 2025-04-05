using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Articles.Comment;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
namespace MagazinEAPI.Models.Users.Readers
{
    public class User : Role
    {
        public List<Subscription> Subscriptions { get; set; } = []; //represents one-to-many User-Subscription
        public List<Ban> Bans { get; set; } = []; //represents one-to-many User-Ban
        public List<Article> ArticleFavoriteArticles { get; set; } = [];
        public ICollection<Article> OwnedArticles { get; set; } = []; //represents many-to-many User-Article
        public List<FavoriteArticle> FavoriteArticles { get; set; } = []; //represents many-to-many User-Article
        public List<Article> ArticleToReadArticles { get; set; } = [];
        public List<ToReadArticle> ToReadArticles { get; set; } = [];//represents many-to-many User-Article
        public List<Tag> FavouriteTags { get; set; } = [];
        public List<TagUser> FavouriteTagUsers { get; set; } = []; //represents many-to-many User-Tag
        public List<Comment> Comments { get; set; } = []; // 1 to 0..n 
        public List<CommentReport> ReportedComments { get; set; } = []; //one to many
        public List<Comment> LikeComments { get; set; } = [];
        public List<Like> Likes { get; set; } = []; //represents many-to-many User-Comment
        public List<Comment> DislikeComments { get; set; } = [];
        public List<Dislike> Dislikes { get; set; } = []; //represents many-to-many User-Comment
        public ICollection<OwnedArticles> OwnedToArticles { get; set; } = []; //represents many-to-many User-Article

        public UserDTO ToDTO()
        {
            return new UserDTO
            {
                ArticlesToReadIds = ArticleToReadArticles.Select(x => x.Id).ToList(),
                FavouriteTagsIds = FavouriteTags.Select(x => x.Id).ToList(),
                CommentsIds = Comments.Select(x => x.Id).ToList(),
                LikedCommentsIds = LikeComments.Select(x => x.Id).ToList(),
                UnlikedCommentsIds = DislikeComments.Select(x => x.Id).ToList(),
                FavouriteArticlesIds = ArticleFavoriteArticles.Select(x => x.Id).ToList(),
                SubscriptionState = Subscriptions.Any(s => s.State == SubscriptionState.Active),
                PersonInfoId = ApplicationUserId,
                Id = Id,

            };
        }
    }
}
