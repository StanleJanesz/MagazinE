using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Admins;
using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models.Requests
{

    public class UnbanRequest : UnbanRequestAbstract
    {
        public Ban Ban { get; set; } = null!; //has 1 Ban (1 to many)
        public int? SolvedById { get; set; } //has 1 Admin
        public Admin? SolvedBy { get; set; } = null!; //has 1 Admin (1 to many)


    }

}
