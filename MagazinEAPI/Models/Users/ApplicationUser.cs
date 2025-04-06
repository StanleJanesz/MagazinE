using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.Models.Users.Journalists;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserState State { get; set; }
        public User? User { get; set; }
        public Editor? Editor { get; set; } 
        public Journalist? Journalist { get; set; }
        public HeadEditor? HeadEditor { get; set; }
        public Admin? Admin { get; set; }


    }
}
