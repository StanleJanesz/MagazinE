using MagazinEAPI.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;




namespace MagazinEAPI.Contexts.Configurations.BanConfigurations
{
    public class BanEntityConfiguration : IEntityTypeConfiguration<Ban>
    {
        public void Configure(EntityTypeBuilder<Ban> builder)
        {
            builder.HasOne(b => b.Admin)
                .WithMany(a => a.Bans)
                .HasForeignKey(b => b.AdminId)
                .IsRequired(); // A Ban must always have an Admin

            builder.HasOne(b => b.User)
                .WithMany(a => a.Bans)
                .HasForeignKey(b => b.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);// A Ban must always have a User

            builder.HasMany(b => b.UnbanRequests)
                .WithOne(a => a.Ban)
                .HasForeignKey(b => b.BanId)
                .IsRequired(); // A Ban must always have an UnbanRequest
        }
    }

}
