using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.Models.Users.Journalists;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models.Users
{
    public class RoleBasedApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserState State { get; set; }

        RoleBasedApplicationUser()
        {
            State = UserState.Active;
        }

    }
}
