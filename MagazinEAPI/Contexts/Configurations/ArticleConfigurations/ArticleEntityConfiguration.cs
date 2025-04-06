using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users.Readers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagazinEAPI.Contexts.Configurations.ArticleConfigurations
{
    public class ArticleEntityConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {

            // One-to-Many: Article → Journalist
            builder.HasOne(b => b.Author)
            .WithMany(a => a.Articles)
            .HasForeignKey(b => b.AuthorId)
            .IsRequired(); // An Article must always have an Author

            builder.HasMany(a => a.Comments)
                .WithOne(b => b.Article)
                .HasForeignKey(b => b.ArticleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // When an Article is deleted, all its Comments are deleted



            builder.HasOne(a => a.Reviewer)
                .WithMany(b => b.Articles)
                .HasForeignKey(a => a.ReviewerId)
                .IsRequired(false);
            builder.HasMany(a => a.PublishRequests)
                .WithOne(b => b.Article)
                .HasForeignKey(b => b.ArticleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // When an Article is deleted, all its PublishRequests are deleted   

            builder.HasMany(a => a.Photos)
                .WithMany(b => b.Articles)
                .UsingEntity<PhotoArticle>(
                                   j => j.HasOne(pa => pa.Photo)
                                          .WithMany(p => p.PhotoArticles)
                                            .HasForeignKey(pa => pa.PhotoId),
                                   j => j.HasOne(pa => pa.Article)
                                         .WithMany(a => a.PhotoArticles)
                                          .HasForeignKey(pa => pa.ArticleId),
                                   j =>
                                   {
                                       j.HasKey(pa => new { pa.ArticleId, pa.PhotoId });
                                   });
            builder.HasMany(a => a.Tags)
                .WithMany(b => b.Articles)
                .UsingEntity<TagArticle>(
                                     j => j.HasOne(ta => ta.Tag)
                                                       .WithMany(t => t.TagArticles)
                                                         .HasForeignKey(ta => ta.TagId),
                                     j => j.HasOne(ta => ta.Article)
                                                       .WithMany(a => a.TagArticles)
                                                         .HasForeignKey(ta => ta.ArticleId),
                                      j =>
                                      {
                                          j.HasKey(ta => new { ta.ArticleId, ta.TagId });
                                      });

            builder.HasMany(a => a.UserFavoriteArticles)
                .WithMany(b => b.ArticleFavoriteArticles)
                .UsingEntity<FavoriteArticle>(
                                    j => j.HasOne(fa => fa.User)
                                          .WithMany(u => u.FavoriteArticles)
                                          .HasForeignKey(fa => fa.UserId),
                                   j => j.HasOne(fa => fa.Article)
                                           .WithMany(a => a.FavoriteArticles)
                                             .HasForeignKey(fa => fa.ArticleId),
                                  j =>
                                  {
                                      j.HasKey(fa => new { fa.ArticleId, fa.UserId });
                                  });
            builder.HasMany(a => a.UserToReadArticles)
                .WithMany(b => b.ArticleToReadArticles)
                .UsingEntity<ToReadArticle>(
                         j => j.HasOne(tra => tra.User)
                               .WithMany(u => u.ToReadArticles)
                               .HasForeignKey(tra => tra.UserId),
                         j => j.HasOne(tra => tra.Article)
                               .WithMany(a => a.ToReadArticles)
                                 .HasForeignKey(tra => tra.ArticleId),
                        j =>
                         {
                             j.HasKey(tra => new { tra.ArticleId, tra.UserId });
                         });


        }
    }
}
