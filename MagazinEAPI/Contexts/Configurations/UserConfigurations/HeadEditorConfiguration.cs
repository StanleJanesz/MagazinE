using MagazinEAPI.Models.Users.Editors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MagazinEAPI.Contexts.Configurations.UserConfigurations
{
    public class HeadEditorConfiguration : IEntityTypeConfiguration<HeadEditor>
    {
        public void Configure(EntityTypeBuilder<HeadEditor> builder)
        {
            builder.HasMany(e => e.EditorsUnder)   
                .WithOne(e => e.HeadEditor)
                .HasForeignKey(e => e.Id)
                .IsRequired(false); // A HeadEditor can have 0...n Editors

            builder.HasOne(e => e.ApplicationUser)
                .WithOne(a => a.HeadEditor)
                .HasForeignKey<HeadEditor>(e => e.ApplicationUserId)
                .IsRequired(); // A HeadEditor must always have an ApplicationUser
        }   
    }
}
