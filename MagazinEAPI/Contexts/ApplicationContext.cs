using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Threading;

using SharedLibrary.Base_Classes___Database;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Users.Journalists;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.Models.Requests;

namespace MagazinEAPI.Contexts
{
    public class APIContext : IdentityDbContext<ApplicationUser>
    {
        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<Dislike> Dislikes { get; set; }
        public DbSet<Editor> Editors { get; set; }
        public DbSet<FavoriteArticle> FavoriteArticles { get; set; }
        public DbSet<HeadEditor> HeadEditors { get; set; }
        public DbSet<Journalist> Journalists { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Photo> PhotoArticles { get; set; }
        public DbSet<PublishRequest> PublishRequests { get; set; }
        public DbSet<RegisterRequest> RegisterRequests { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagArticle> TagArticles { get; set; }
        public DbSet<TagEditor> TagEditors { get; set; }
        public DbSet<TagUser> TagUsers { get; set; }
        public DbSet<ToReadArticle> ToReadArticles { get; set; }
        public DbSet<UnbanRequest> UnbanRequests { get; set; }
        public DbSet<User> Readers { get; set; } //USERS było zajęte


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //TU BĘDZIE TURBO DUZO UZUPEŁNIANIA


            //User has one applicationUser but applicationUser have zero or one user
            modelBuilder.Entity<ApplicationUser>()
            .HasOne(au => au.User)
            .WithOne(u => u.ApplicationUser)
            .HasForeignKey<User>(u => u.ApplicationUserId)
            .IsRequired();

            //Editor has one applicationUser but applicationUser have zero or one editor
            modelBuilder.Entity<ApplicationUser>()
            .HasOne(au => au.Editor)
            .WithOne(e => e.ApplicationUser)
            .HasForeignKey<Editor>(e => e.ApplicationUserId)
            .IsRequired();

            //Journalist has one applicationUser but applicationUser have zero or one Journalist
            modelBuilder.Entity<ApplicationUser>()
            .HasOne(au => au.Journalist)
            .WithOne(j => j.ApplicationUser)
            .HasForeignKey<Journalist>(j => j.ApplicationUserId)
            .IsRequired();

            //HeadEditor has one applicationUser but applicationUser have zero or one HeadEditor
            modelBuilder.Entity<ApplicationUser>()
            .HasOne(au => au.HeadEditor)
            .WithOne(j => j.ApplicationUser)
            .HasForeignKey<HeadEditor>(j => j.ApplicationUserId)
            .IsRequired();

            //Admin has one applicationUser but applicationUser have zero or one Admin
            modelBuilder.Entity<ApplicationUser>()
            .HasOne(au => au.Admin)
            .WithOne(j => j.ApplicationUser)
            .HasForeignKey<Admin>(j => j.ApplicationUserId)
            .IsRequired();


            // One-to-Many: Admin → Ban
            modelBuilder.Entity<Ban>()
            .HasOne(b => b.Admin)
            .WithMany(a => a.Bans)
            .HasForeignKey(b => b.AdminId)
            .IsRequired(); // A Ban must always have an Admin

            //one/zero to many
            modelBuilder.Entity<Admin>()
            .HasMany(e => e.DeletedComments)
            .WithOne(e => e.DeletedBy)
            .HasForeignKey(e => e.DeletedById)
            .IsRequired(false);

            //one/zero to many
            modelBuilder.Entity<Admin>()
            .HasMany(e => e.ManagedCommentReports)
            .WithOne(e => e.ManagedBy)
            .HasForeignKey(e => e.ManagedById)
            .IsRequired(false);


            // One-to-Many: Article → Journalist
            modelBuilder.Entity<Article>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Articles)
            .HasForeignKey(b => b.AuthorId)
            .IsRequired(); // An Article must always have an Author

            // One-to-Many: Comment → Article
            modelBuilder.Entity<Article>()
            .HasMany(a => a.Comments)
            .WithOne(b => b.Article)
            .HasForeignKey(b => b.ArticleId)
            .IsRequired(); // A Comment must always have an Article

            //may-to-many Tag-Article with TagArticle join table
            modelBuilder.Entity<Article>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Articles)
            .UsingEntity<TagArticle>(
                l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.TagArticles),
                r => r.HasOne<Article>(e => e.Article).WithMany(e => e.TagArticles));

            //may-to-many Photo-Article with PhotoArticle join table
            modelBuilder.Entity<Article>()
            .HasMany(e => e.Photos)
            .WithMany(e => e.Articles)
            .UsingEntity<PhotoArticle>(
                l => l.HasOne<Photo>(e => e.Photo).WithMany(e => e.PhotoArticles),
                r => r.HasOne<Article>(e => e.Article).WithMany(e => e.PhotoArticles));

            /*
						// One-to-Many: Photo → Article
						modelBuilder.Entity<Article>()
						.HasMany(a => a.Photos)
						.WithOne(b => b.Article)
						.HasForeignKey(b => b.ArticleId)
						.IsRequired(); // A Photo must always have an Article
			*/

            // One-to-Many: PublishRequest → Article
            modelBuilder.Entity<Article>()
            .HasMany(a => a.PublishRequests)
            .WithOne(b => b.Article)
            .HasForeignKey(b => b.ArticleId)
            .IsRequired(); // A PublishRequest must always have an Article

            //one/zero to many : Article -> Editor
            modelBuilder.Entity<Article>()
            .HasOne(e => e.Reviewer)
            .WithMany(e => e.Articles)
            .HasForeignKey(e => e.ReviewerId)
            .IsRequired(false); //Article may have an Editor

            //may-to-many User-Article with FavouriteArticle join table
            modelBuilder.Entity<Article>()
            .HasMany(e => e.UserFavoriteArticles)
            .WithMany(e => e.ArticleFavoriteArticles)
            .UsingEntity<FavoriteArticle>(
                l => l.HasOne<User>(e => e.User).WithMany(e => e.FavoriteArticles),
                r => r.HasOne<Article>(e => e.Article).WithMany(e => e.FavoriteArticles));

            //may-to-many User-Article with ToRead join table
            modelBuilder.Entity<Article>()
            .HasMany(e => e.UserToReadArticles)
            .WithMany(e => e.ArticleToReadArticles)
            .UsingEntity<ToReadArticle>(
                l => l.HasOne<User>(e => e.User).WithMany(e => e.ToReadArticles),
                r => r.HasOne<Article>(e => e.Article).WithMany(e => e.ToReadArticles));

            // One-to-Many: User → Ban
            modelBuilder.Entity<Ban>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bans)
            .HasForeignKey(b => b.UserId)
            .IsRequired(); // A Ban must always have a User

            // One-to-Many: UnbanRequest → Ban
            modelBuilder.Entity<Ban>()
            .HasMany(a => a.UnbanRequests)
            .WithOne(b => b.Ban)
            .HasForeignKey(b => b.BanId)
            .IsRequired(); // A UnbanRequest must always have a Ban

            // One-to-Many: Comment → User
            modelBuilder.Entity<Comment>()
            .HasOne(a => a.Author)
            .WithMany(b => b.Comments)
            .HasForeignKey(b => b.AuthorId)
            .IsRequired(); // A Comment must always have a User

            //may-to-many User-Comment with Likes join table
            modelBuilder.Entity<Comment>()
            .HasMany(e => e.LikeUsers)
            .WithMany(e => e.LikeComments)
            .UsingEntity<Like>(
                l => l.HasOne<User>(e => e.User).WithMany(e => e.Likes),
                r => r.HasOne<Comment>(e => e.Comment).WithMany(e => e.Likes));

            //may-to-many User-Comment with Dislikes join table
            modelBuilder.Entity<Comment>()
            .HasMany(e => e.DislikeUsers)
            .WithMany(e => e.DislikeComments)
            .UsingEntity<Dislike>(
                l => l.HasOne<User>(e => e.User).WithMany(e => e.Dislikes),
                r => r.HasOne<Comment>(e => e.Comment).WithMany(e => e.Dislikes));


            //
            //
            //   PARENT-CHILD RELATIONSHIP WITH COMMENTS
            //
            //
            modelBuilder.Entity<Comment>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .IsRequired(false);


            // One-to-Many: CommentReport → Comment
            modelBuilder.Entity<Comment>()
            .HasMany(a => a.Reports)
            .WithOne(b => b.Comment)
            .HasForeignKey(b => b.CommentId)
            .IsRequired(); // A CommentReport must always have a Comment

            // One-to-Many: CommentReport → User
            modelBuilder.Entity<CommentReport>()
            .HasOne(a => a.ReportAuthor)
            .WithMany(b => b.ReportedComments)
            .HasForeignKey(b => b.ReportAuthorId)
            .IsRequired(); // A CommentReport must always have an User

            // One-to-Many: Editor → HeadEditor
            modelBuilder.Entity<Editor>()
            .HasOne(a => a.HeadEditor)
            .WithMany(b => b.EditorsUnder)
            .HasForeignKey(b => b.HeadEditorId)
            .IsRequired(); // An Editor must always have an HeadEditor

            //may-to-many Tag-Editor with TagEditor join table
            modelBuilder.Entity<Editor>()
            .HasMany(e => e.AllowedTags)
            .WithMany(e => e.Editors)
            .UsingEntity<TagEditor>(
                l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.TagEditors),
                r => r.HasOne<Editor>(e => e.Editor).WithMany(e => e.AllowedTagEditors));

            //one/zero to many : PublishRequest -> Editor
            modelBuilder.Entity<Editor>()
            .HasMany(e => e.PublishRequests)
            .WithOne(e => e.Reviewer)
            .HasForeignKey(e => e.ReviewerId)
            .IsRequired(false); //PublishRequest may have an Editor

            // One-to-Many: Journalist → HeadEditor
            modelBuilder.Entity<Journalist>()
            .HasOne(a => a.HeadEditor)
            .WithMany(b => b.JournalistsUnder)
            .HasForeignKey(b => b.HeadEditorId)
            .IsRequired(); // A Journalist must always have a HeadEditor

            // One-to-Many: Subscription → User
            modelBuilder.Entity<Subscription>()
            .HasOne(a => a.User)
            .WithMany(b => b.Subscriptions)
            .HasForeignKey(b => b.UserId)
            .IsRequired(); // A Subscription must always have a User

            //may-to-many Tag-User with TagUser join table
            modelBuilder.Entity<Tag>()
            .HasMany(e => e.Users)
            .WithMany(e => e.FavouriteTags)
            .UsingEntity<TagUser>(
                l => l.HasOne<User>(e => e.User).WithMany(e => e.FavouriteTagUsers),
                r => r.HasOne<Tag>(e => e.Tag).WithMany(e => e.TagUsers));
        }
    }
}