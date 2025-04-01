using MagazinEAPI.Models.Requests;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
using System.Security.Cryptography.Pkcs;
namespace MagazinEAPI.Models.Users
{
    public class Ban : BanAbstract
    {
        public int AdminId { get; set; } //has 1 Admin
        public Admin Admin { get; set; } = null!;
        public int UserId { get; set; } //has 1 User
        public User User { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public string Reason { get; set; } = null!;
        public List<UnbanRequest> UnbanRequests { get; set; } = []; //one to many
    }
}
