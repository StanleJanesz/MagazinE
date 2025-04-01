using MagazinEAPI.Models.Articles.Comment;
using MagazinEAPI.Models.Users.Readers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace MagazinEAPI.Contexts.Configurations.ArticleConfigurations.CommentsConfigurations
{
    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(c => c.Author)
                 .WithMany(u => u.Comments)
                 .HasForeignKey(c => c.AuthorId)
                 .IsRequired(); // A Comment must always have a User
            builder.HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId)
                .IsRequired(); // A Comment must always have an Article
            builder.HasMany(c => c.Likes)
                .WithOne(l => l.Comment)
                .HasForeignKey(c => c.CommentId)
                .IsRequired(); // A Comment must always have a Like

            builder.HasMany(c => c.Dislikes)
                .WithOne(d => d.Comment)
                .HasForeignKey(c => c.CommentId)
                .IsRequired(); // A Comment must always have a Dislike

            builder.HasMany(c => c.Reports)
                .WithOne(r => r.Comment)
                .HasForeignKey(c => c.CommentId)
                .IsRequired(); // A Comment must always have a Report

            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false); // A Comment may have a Parent

            builder.HasOne(c => c.DeletedBy)
                .WithMany(a => a.DeletedComments)
                .HasForeignKey(c => c.DeletedById)
                .IsRequired(false); // A Comment may have a DeletedBy

            builder.HasMany(c => c.LikeUsers)
                .WithMany(u => u.LikeComments)
                .UsingEntity<Like>(
                   l => l.HasOne<User>(e => e.User).WithMany(e => e.Likes).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasOne<Comment>(e => e.Comment).WithMany(e => e.Likes).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasKey(pa => new { pa.CommentId, pa.UserId }));

            builder.HasMany(c => c.DislikeUsers)
                .WithMany(u => u.DislikeComments)
                .UsingEntity<Dislike>(
                   l => l.HasOne<User>(e => e.User).WithMany(e => e.Dislikes).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasOne<Comment>(e => e.Comment).WithMany(e => e.Dislikes).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasKey(pa => new { pa.CommentId, pa.UserId }));

        }
    }
}
