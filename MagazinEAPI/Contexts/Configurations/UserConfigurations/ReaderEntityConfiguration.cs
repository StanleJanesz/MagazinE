using MagazinEAPI.Models.Users.Readers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace MagazinEAPI.Contexts.Configurations.UserConfigurations
{
    public class ReaderEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(r => r.ApplicationUser)
                .WithOne(a => a.User)
                .HasForeignKey<User>(r => r.ApplicationUserId)
                .IsRequired(); // A Reader must always have an ApplicationUser


            builder.HasMany(r => r.ReportedComments)
                .WithOne(rc => rc.ReportAuthor)
                .HasForeignKey(rc => rc.ReportAuthorId)
                .IsRequired(false); // A Reader can have 0...n ReportedComments


            builder.HasMany(r => r.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .IsRequired(); // A Reader must always have a Subscription

            builder.HasMany(r => r.FavouriteTags)
                .WithMany(t => t.Users)
                .UsingEntity<TagUser>(

                j => j.HasOne(tu => tu.Tag)
                      .WithMany(t => t.TagUsers)
                      .HasForeignKey(tu => tu.TagId),
                j => j.HasOne(tu => tu.User)
                      .WithMany(u => u.FavouriteTagUsers)
                      .HasForeignKey(tu => tu.UserId),
                j =>
                {
                    j.HasKey(tu => new { tu.TagId, tu.UserId });
                }
                ); // A Reader can have 1...n Tags
            builder.HasMany(r => r.OwnedArticles)
                 .WithMany(a => a.UserOwnedArticles)
                 .UsingEntity<OwnedArticles>(
                j => j.HasOne(oa => oa.Article)
                      .WithMany(a => a.OwnedToArticles)
                      .HasForeignKey(oa => oa.ArticleId),
                j => j.HasOne(oa => oa.User)
                .WithMany(u => u.OwnedToArticles)
                        .HasForeignKey(oa => oa.UserId),
                j => j.HasKey(oa => new { oa.ArticleId, oa.UserId })
                ); 





        }
    }
}
