using MagazinEAPI.Models.Users.Editors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MagazinEAPI.Contexts.Configurations.UserConfigurations
{
    public class EditorEntityConfiguration : IEntityTypeConfiguration<Editor>
    {
        public void Configure(EntityTypeBuilder<Editor> builder)
        {
            

            builder.HasMany(e => e.Articles)
                .WithOne(a => a.Reviewer)
                .HasForeignKey(e => e.ReviewerId)
                .IsRequired(false); // An Editor can have 0...n Articles

            builder.HasMany(e => e.AllowedTags)
                .WithMany(t => t.Editors)
                .UsingEntity<TagEditor>(
                j => j.HasOne(te => te.Tag)
                      .WithMany(t => t.TagEditors)
                      .HasForeignKey(te => te.TagId),
              j => j.HasOne(te => te.Editor)
                       .WithMany(e => e.AllowedTagEditors)
                        .HasForeignKey(te => te.EditorId),
                j =>
                {
                    j.HasKey(te => new { te.TagId, te.EditorId });
                }
                ); // An Editor can have 1...n Tags

            builder.HasMany(e => e.PublishRequests)
                .WithOne(p => p.Reviewer)
                .HasForeignKey(e => e.ReviewerId)
                .IsRequired(false); // An Editor can have 0...n PublishRequests

            builder.HasOne(e => e.ApplicationUser)
                .WithOne(a => a.Editor)
                .HasForeignKey<Editor>(e => e.ApplicationUserId)
                .IsRequired(); // An Editor must always have an ApplicationUser

        }


    }
}
