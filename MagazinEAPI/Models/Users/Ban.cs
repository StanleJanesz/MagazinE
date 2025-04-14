using MagazinEAPI.Models.Requests;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
using System.Security.Cryptography.Pkcs;
using SharedLibrary.DTO_Classes;
namespace MagazinEAPI.Models.Users
{
    public class Ban : BanAbstract
    {
        public Admin Admin { get; set; } = null!;
        public User User { get; set; } = null!;
        public List<UnbanRequest> UnbanRequests { get; set; } = []; //one to many

        public BanDTO toDTO()
        {
            return new BanDTO()
            {
                Id = this.Id,
                UserId = this.UserId,
                AdminId = this.AdminId,
                Active = this.Active,
                BanStartDate = this.BanStartDate,
                BanEndDate = this.BanEndDate,
                UnbanRequestIds = this.UnbanRequests.Select(ur => ur.Id).ToList()
            };
        }
    }
}
