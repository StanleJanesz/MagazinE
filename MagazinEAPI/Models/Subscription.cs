using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models
{
	public class Subscription : SubscriptionAbstract
	{
		public User User { get; set; } = null!; //has 1 user
	}
}
