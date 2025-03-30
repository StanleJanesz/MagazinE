using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using MagazinEAPI.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MagazinEAPI.Contexts;

namespace MagazinEAPI.utils.SeedCreators
{
    public static class RolesSeed
    {
        public static void InitializeRoles(ModelBuilder modelBuilder) // only on Creation 
        {
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Description = "Admin", Name = "Admin", NormalizedName = "ADMIN".ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Description = "Reader", Name = "Reader", NormalizedName = "Reader".ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7212", Description = "Editor", Name = "Editor", NormalizedName = "Editor".ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7213", Description = "HeadEditor", Name = "HeadEditor", NormalizedName = "HeadEditor".ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7214", Description = "Journalist", Name = "Journalist", NormalizedName = "Journalist".ToUpper() });
        }

    }
}
