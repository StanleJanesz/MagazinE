namespace MagazinEAPI.Models
{

	public abstract class SubscriptionAbstract
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SubscriptionState State { get; set; }
	} 

	public class SubscriptionDTO : SubscriptionAbstract
	{

	}

	public class Subscription : SubscriptionAbstract
	{
		public User User { get; set; } = null!; //has 1 user
	}
}
