using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models
{
	public class Ban : BanAbstract
	{
		public Admin Admin { get; set; } = null!;
		public User User { get; set; } = null!;

		public List<UnbanRequest> UnbanRequests { get; set; } = []; //one to many
	}
}
