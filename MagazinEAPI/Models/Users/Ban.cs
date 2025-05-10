using MagazinEAPI.Models.Requests;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Readers;
using SharedLibrary.Base_Classes___Database;
using System.Security.Cryptography.Pkcs;
using SharedLibrary.DTO_Classes;
namespace MagazinEAPI.Models.Users
{
    /// <summary>
    /// Ban class represents a ban on a user.
    /// Stored in the database.
    /// </summary>
    public class Ban : BanAbstract
    {
        /// <summary>
        /// Gets or Sets admin who issued the ban.
        /// </summary>
        public Admin Admin { get; set; } = null!;
        
        /// <summary>
        /// Gets or Sets user who is banned.
        /// </summary>
        public User User { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets list of unban requests related to this ban.
        /// </summary>
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
