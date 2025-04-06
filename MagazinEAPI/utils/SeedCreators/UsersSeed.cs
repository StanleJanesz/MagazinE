using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Journalists;
using MagazinEAPI.Models.Users.Readers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MagazinEAPI.utils.SeedCreators
{
    public static class UsersSeed  // FOR TESTING PURPOSES ONLY
    {

        public static void InitializeUsers(ModelBuilder modelBuilder)
        {
            CreateAdmin(modelBuilder);

            CreateHeadEditor(modelBuilder);

            CreateEditor(modelBuilder);

            CreateJournalist(modelBuilder);

            CreateReader(modelBuilder);
        }






        static void CreateAdmin(ModelBuilder modelBuilder)
        {
            var adminRoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210";
            var adminId = "2c5w174e-3b0e-446f-86af-483d56fd7214";

            var admin = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                SecurityStamp = "c5ba1859-83da-4fed-b0b8-965f2b80d3ea",
                ConcurrencyStamp = "599c3aa8-eda2-4861-a51d-5c6ba68596c0",
            };
            var hasher = new PasswordHasher<ApplicationUser>();

            admin.PasswordHash = "AQAAAAIAAYagAAAAEGjtaYT1QQugEtDzxCHL5AD80LCJ9rX+EAYl44HijtqzIHZXfphoRhNjjBW2WujvIw=="; // hasher.HashPassword(admin, "Pass123!");
            

            modelBuilder.Entity<ApplicationUser>().HasData(admin);


            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                ApplicationUserId = adminId
            });


            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminId
            });
        }


        static void CreateHeadEditor(ModelBuilder modelBuilder)
        {

            var HeadEditorRoleId = "2c5e174e-3b0e-446f-86af-483d56fd7213";
            var HeadEditorId = "2c5w174e-3b0e-486f-86af-483d56fd7213";

            var headEditor = new ApplicationUser
            {
                Id = HeadEditorId,
                UserName = "HeadEditor",
                Email = "headeditor@example.com",
                NormalizedEmail = "headeditor@example.com".ToUpper(),
                NormalizedUserName = "HeadEditor".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = "dd7090c7-30a5-46d5-9e05-815f38bbd6ab",
                ConcurrencyStamp = "dd2d325e-0c77-4a07-9b08-42ed37020384",
            };

            var hasher = new PasswordHasher<ApplicationUser>();

            headEditor.PasswordHash = "AQAAAAIAAYagAAAAENKzyCo5g+I320Z15/ZxFZopnss/ggALZ73kN4z2QWrVG+NRZz9bk7L+w7dSBYa4Ag=="; //hasher.HashPassword(headEditor, "Pass123!");

            modelBuilder.Entity<ApplicationUser>().HasData(headEditor);

            modelBuilder.Entity<HeadEditor>().HasData(new HeadEditor
            {
                Id = 1,
                ApplicationUserId = HeadEditorId,

            });
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = HeadEditorRoleId,
                UserId = HeadEditorId
            });


        }
        static void CreateEditor(ModelBuilder modelBuilder)
        {
            var EditorRoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212";
            var EditorId = "2c5w179e-3b0e-446f-86af-483d56fd7212";

            var Editor = new ApplicationUser
            {
                Id = EditorId,
                UserName = "Editor",
                Email = "editor@example.com",
                NormalizedEmail = "editor@example.com".ToUpper(),
                NormalizedUserName = "Editor".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = "dd709037-30a5-46d5-9e05-815f38bbd6ab",
                ConcurrencyStamp = "dd2d323e-0c77-4a07-9b08-42ed37020384",

            };

            var hasher = new PasswordHasher<ApplicationUser>();

            Editor.PasswordHash = "AQAAAAIAAYagAAAAEDaQJNcmsFaqlvYpylJ8LL9S9aYOCTsB7PtdbsCQsymITYP5lz9PK77Q3FJw+xFmGw==";  //hasher.HashPassword(Editor, "Pass123!");
            modelBuilder.Entity<ApplicationUser>().HasData(Editor);

            modelBuilder.Entity<Editor>().HasData(new Editor
            {
                Id = 1,
                ApplicationUserId = EditorId,
                HeadEditorId = 1,
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = EditorRoleId,
                UserId = EditorId,

            });
        }

        static void CreateJournalist(ModelBuilder modelBuilder)
        {
            var JournalistRoleId = "2c5e174e-3b0e-446f-86af-483d56fd7214";
            var JournalistId = "2c5w174r-3b0e-446f-86af-483d56fd7214";

            var Journalist = new ApplicationUser
            {
                Id = JournalistId,
                UserName = "Journalist",
                Email = "journalist@example.com",
                NormalizedEmail = "journalist@example.com".ToUpper(),
                NormalizedUserName = "Journalist".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = "dd709037-30a5-46d5-9e05-815f38b6d6ab",
                ConcurrencyStamp = "dd2d323e-0c77-4a07-9b08-42ed38020384",

            };

            var hasher = new PasswordHasher<ApplicationUser>();

            Journalist.PasswordHash = "AQAAAAIAAYagAAAAEOAXkcsy7IiVg45vv09dUPqtwHOzbwnO1kOLG7lC4iVA9G8VOr2qjWxBcGtQN1zC+w=="; // hasher.HashPassword(Journalist, "Pass123!");

            modelBuilder.Entity<ApplicationUser>().HasData(Journalist);

            modelBuilder.Entity<Journalist>().HasData(new Journalist
            {
                Id = 1,
                ApplicationUserId = JournalistId,
                HeadEditorId = 1,
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = JournalistRoleId,
                UserId = JournalistId
            });
        }

        static void CreateReader(ModelBuilder modelBuilder)
        {
            var ReaderRoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211";
            var ReaderId = "2c5w174r-3b0e-446f-86af-483d56fd7211";

            var reader = new ApplicationUser
            {
                Id = ReaderId,
                UserName = "reader",
                Email = "reader@example.com",
                NormalizedEmail = "reader@example.com".ToUpper(),
                NormalizedUserName = "reader".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = "dd709037-30a5-46d5-9e05-515f38b6d6ab",
                ConcurrencyStamp = "dd2d323e-0c77-4a07-9b08-422d38020384",

            };

            var hasher = new PasswordHasher<ApplicationUser>();

            reader.PasswordHash = "AQAAAAIAAYagAAAAECAtlY2RA/vjLYvmweL9IBRZJl25Few+FUY8QMsC3/rJADAsQxzWxlD4QR4/ZJoSAw==";// hasher.HashPassword(reader, "Pass123!");

            modelBuilder.Entity<ApplicationUser>().HasData(reader);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                ApplicationUserId = ReaderId,
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ReaderRoleId,
                UserId = ReaderId,

            });
        }


    }
}
