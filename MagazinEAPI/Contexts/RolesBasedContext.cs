using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Journalists;
using MagazinEAPI.Models.Users.Readers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Base_Classes___Database;
using MagazinEAPI.Contexts.Configurations;
using MagazinEAPI.Contexts.Configurations.ArticleConfigurations;
using MagazinEAPI.Contexts.Configurations.ArticleConfigurations.CommentsConfigurations;
using MagazinEAPI.Contexts.Configurations.UserConfigurations;
using MagazinEAPI.Contexts.Configurations.BanConfigurations;
using MagazinEAPI.utils.SeedCreators;
using MagazinEAPI.Models.Requests;



namespace MagazinEAPI.Contexts
{
    public class RolesBasedContext : IdentityDbContext<ApplicationUser, ApplicationRole,string>
    {
        public RolesBasedContext(DbContextOptions<RolesBasedContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }

        public virtual DbSet<Article> Articles { get; set; }

        public virtual DbSet<Ban> Bans { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<CommentReport> CommentReports { get; set; }

        public virtual DbSet<Dislike> Dislikes { get; set; }

        public virtual DbSet<Editor> Editors { get; set; }

        public virtual DbSet<FavoriteArticle> FavoriteArticles { get; set; }

        public virtual DbSet<HeadEditor> HeadEditors { get; set; }

        public virtual DbSet<Journalist> Journalists { get; set; }

        public virtual DbSet<Like> Likes { get; set; }

        public virtual DbSet<Photo> Photos { get; set; }

        public virtual DbSet<PhotoArticle> PhotoArticles { get; set; }

        public virtual DbSet<PublishRequest> PublishRequests { get; set; }

        public virtual DbSet<RegisterRequest> RegisterRequests { get; set; }

        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<TagArticle> TagArticles { get; set; }

        public virtual DbSet<TagEditor> TagEditors { get; set; }

        public virtual DbSet<TagUser> TagUsers { get; set; }

        public virtual DbSet<ToReadArticle> ToReadArticles { get; set; }

        public virtual DbSet<UnbanRequest> UnbanRequests { get; set; }

        public virtual DbSet<User> Readers { get; set; } //USERS było zajęte


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            new BanEntityConfiguration().Configure(modelBuilder.Entity<Ban>());

            new ArticleEntityConfiguration().Configure(modelBuilder.Entity<Article>());

            new CommentEntityConfiguration().Configure(modelBuilder.Entity<Comment>());

            new ReaderEntityConfiguration().Configure(modelBuilder.Entity<User>());

            new AdminEntityConfiguration().Configure(modelBuilder.Entity<Admin>());

            new HeadEditorConfiguration().Configure(modelBuilder.Entity<HeadEditor>());

            new EditorEntityConfiguration().Configure(modelBuilder.Entity<Editor>());

            new JouranlistEntityConfiguration().Configure(modelBuilder.Entity<Journalist>());

            RolesSeed.InitializeRoles(modelBuilder);

            UsersSeed.InitializeUsers(modelBuilder); // FOR TESTING PURPOSES ONLY
        }
    }
}
