using MagazinEAPI.Models.Users.Admins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace MagazinEAPI.Contexts.Configurations.UserConfigurations
{
    public class AdminEntityConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasMany(a => a.Bans)
                .WithOne(b => b.Admin)
                .HasForeignKey(b => b.AdminId)
                .IsRequired(); //one-to-many


            builder.HasMany(a => a.DeletedComments)
                .WithOne(b => b.DeletedBy)
                .HasForeignKey(b => b.DeletedById)
                .IsRequired(false); //one-to-many


            builder.HasMany(a => a.ManagedCommentReports)
                .WithOne(b => b.ManagedBy)
                .HasForeignKey(b => b.ManagedById)
                .IsRequired(false); //one-to-many

            builder.HasMany(a => a.SolvedUnbanRequests)
                .WithOne(b => b.SolvedBy)
                .HasForeignKey(b => b.SolvedById)
                .IsRequired(false); //one-to-many

            builder.HasOne(a => a.ApplicationUser)
                .WithOne(b => b.Admin)
                .HasForeignKey<Admin>(b => b.ApplicationUserId)
                .IsRequired(); //one-to-one
        }

    }
}
