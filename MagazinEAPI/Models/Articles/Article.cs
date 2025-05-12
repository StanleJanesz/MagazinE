namespace MagazinEAPI.Models.Articles
{
    using MagazinEAPI.Contexts;
    using MagazinEAPI.Models.Requests;
    using MagazinEAPI.Models.Users;
    using MagazinEAPI.Models.Users.Editors;
    using MagazinEAPI.Models.Users.Journalists;
    using MagazinEAPI.Models.Users.Readers;
    using Microsoft.AspNetCore.Identity;
    using SharedLibrary.Base_Classes___Database;
    using SharedLibrary.DTO_Classes;

    public class Article : ArticleAbstract
    {
        public Journalist Author { get; set; } = null!; //has 1 Author (1 - to - 0..n) -> AuthorId in abtractclass

        public List<Comment.Comment> Comments { get; set; } = []; //has 0...n Comments (1 - to 0..n)

        public int? ReviewerId { get; set; } //has 0...1 reviewers

        public Editor? Reviewer { get; set; } //has 0...1 reviewers (0..1 - to - 0..n)

        public List<PublishRequest> PublishRequests { get; set; } = []; // one article can have many publish request ???

        public List<PhotoArticle> PhotoArticles { get; set; } = [];

        public List<Photo> Photos { get; set; } = []; //many-to-many

        public List<Tag> Tags { get; set; } = [];

        public List<TagArticle> TagArticles { get; set; } = []; //has 1...n Tags (many-to-many)

        public List<User> UserFavoriteArticles { get; set; } = [];

        public List<FavoriteArticle> FavoriteArticles { get; set; } = []; //represents many-to-many User-Article

        public List<User> UserToReadArticles { get; set; } = [];

        public List<ToReadArticle> ToReadArticles { get; set; } = [];//represents many-to-many User-Article

        public ICollection<User> UserOwnedArticles { get; set; } = []; //represents many-to-many User-Article

        public ICollection<OwnedArticles> OwnedToArticles { get; set; } = []; //represents many-to-many User-Article

        public ArticleDTO ToDTO()
        {
            var articleDTO = new ArticleDTO
            {
                Id = Id,
                Title = Title,
                Content = Content,
                isPremium = isPremium,
                isPublished = isPublished,
                Introduction = Introduction,
                TimeOfPublication = TimeOfPublication,
                AuthorId = AuthorId,
                CommentsIds = Comments.Select(c => c.Id).ToList(),
                Photos = Photos.Select(p => p.Content).ToList(),
                TagsIds = Tags.Select(t => t.Id).ToList()
            };
            return articleDTO;
        }

        public ArticleDTO ToInfoDTO() // used to get info about article for main page
        {
            var articleDTO = new ArticleDTO
            {
                Id = Id,
                Title = Title,
                Content = "",
                isPremium = isPremium,
                isPublished = isPublished,
                Introduction = Introduction,
                TimeOfPublication = TimeOfPublication,
                AuthorId = AuthorId,
                Photos = Photos.Take(1).Select(p => p.Content).ToList(),
                TagsIds = Tags.Select(t => t.Id).ToList()
            };
            return articleDTO;
        }
        public bool CanBeViewedBy(ApplicationUser user, RolesBasedContext context, UserManager<ApplicationUser> userManager) // used to check if user can see article
        {
            List<string> roles = userManager.GetRolesAsync(user).Result.ToList();
            var roleVisibilityMap = new Dictionary<string, Func<ApplicationUser, RolesBasedContext, bool>>
            {
                { "Admin", VisibleToAdmin }, // Admin can see all articles
                { "Journalist", VisibleToJournalist }, // Journalist can see only his articles
                { "Reader", VisibleToReader }, // Reader can see all articles if they are free or he is subscribed
                { "Editor", VisibleToEditor }, // Editor can see articles with tags allowed by his HeadEditor
                { "HeadEditor", VisibleToHeadEditor } // HeadEditor can see articles of his Journalists
             };
            return roles.Any(role => roleVisibilityMap.ContainsKey(role) && roleVisibilityMap[role](user, context));
        }

        public bool CanBeDelatedBy(ApplicationUser user, RolesBasedContext context, UserManager<ApplicationUser> userManager) // used to check if user can delete article
        {
            List<string> roles = userManager.GetRolesAsync(user).Result.ToList();
            var roleVisibilityMap = new Dictionary<string, Func<ApplicationUser, RolesBasedContext, bool>>
            {
               // { "Admin", VisibleToAdmin }, // Admin can delete all articles   // TODO: decide if Admin can delete all articles
                { "Journalist", VisibleToJournalist }, // Journalist can delete only his articles 
               // { "Editor", VisibleToEditor }, // Editor can delete articles with tags allowed by his HeadEditor // TODO: I would say that Editor can chage ispublished state but cannot expicit delete article, but up to discussion
                { "HeadEditor", VisibleToHeadEditor } // HeadEditor can delete articles of his Journalists
            };
            return roles.Any(role => roleVisibilityMap.ContainsKey(role) && roleVisibilityMap[role](user, context));
        }

        public bool CanBeEditedBy(ApplicationUser user, RolesBasedContext context, UserManager<ApplicationUser> userManager) // used to check if user can edit article
        {
            List<string> roles = userManager.GetRolesAsync(user).Result.ToList();
            var roleVisibilityMap = new Dictionary<string, Func<ApplicationUser, RolesBasedContext, bool>>
            {
               // { "Admin", VisibleToAdmin }, // preaty sure that Admin can't edit articles
                { "Journalist", VisibleToJournalist }, // Journalist can edit only his articles
                { "Editor", VisibleToEditor }, // Editor can edit articles with tags allowed by his HeadEditor // TODO: 
               // { "HeadEditor", VisibleToHeadEditor } // HeadEditor is main dont edit articles he controls the process of creating articles
            };
            return roles.Any(role => roleVisibilityMap.ContainsKey(role) && roleVisibilityMap[role](user, context));
        }

        private bool VisibleToAdmin(ApplicationUser user, RolesBasedContext _context)
        {
            return true;
        }

        private bool VisibleToJournalist(ApplicationUser user, RolesBasedContext _context)
        {
            Journalist? journalist = _context.Journalists.FirstOrDefault(j => j.ApplicationUserId == user.Id);

            if (journalist == null)
            {
                return false;
            }

            if (journalist.Articles.Any(a => a.Id == Id))
                return true;
            return false;
        }

        private bool VisibleToReader(ApplicationUser user, RolesBasedContext _context)
        {
            User? reader = _context.Readers.FirstOrDefault(j => j.ApplicationUserId == user.Id);
            if (reader == null)
            {
                return false;
            }
            if (!isPublished)
            {
                return false;
            }
            if (isPremium)
            {
                if (reader.OwnedArticles.Contains(this)) return true;

                return reader.Subscriptions.Any(s => s.State == SubscriptionState.Active);
            }
            return true;
        }

        private bool VisibleToEditor(ApplicationUser user, RolesBasedContext _context)
        {
            Editor? editor = _context.Editors.FirstOrDefault(j => j.ApplicationUserId == user.Id);

            if (editor == null)
            {
                return false;
            }
            
            return Tags.All(t => editor.AllowedTags.Contains(t));
        }
        private bool VisibleToHeadEditor(ApplicationUser user, RolesBasedContext _context)
        {
            HeadEditor? headEditor = _context.HeadEditors.FirstOrDefault(j => j.ApplicationUserId == user.Id);

            if (headEditor == null)
            {
                return false;
            }
            return headEditor.JournalistsUnder.Any(
                j => j.Articles.Any(a => a.Id == Id));

        }


    }


}
