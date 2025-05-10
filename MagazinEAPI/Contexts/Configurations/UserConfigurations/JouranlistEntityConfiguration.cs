using MagazinEAPI.Models.Users.Journalists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagazinEAPI.Contexts.Configurations.UserConfigurations
{
    public class JouranlistEntityConfiguration : IEntityTypeConfiguration<Journalist>
    {
        public void Configure(EntityTypeBuilder<Journalist> builder)
        {
            builder.HasOne(j => j.HeadEditor)
                .WithMany(h => h.JournalistsUnder)
                .HasForeignKey(j => j.HeadEditorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // A Journalist must always have a HeadEditor

            builder.HasMany(j => j.Articles)
                .WithOne(a => a.Author)
                .HasForeignKey(j => j.AuthorId)
                .IsRequired(false); // A Journalist can have 0...n Articles

            builder.HasOne(j => j.ApplicationUser)
                .WithOne(a => a.Journalist)
                .HasForeignKey<Journalist>(j => j.ApplicationUserId)
                .IsRequired(); // A Journalist must always have an ApplicationUser
        }
    }
}
