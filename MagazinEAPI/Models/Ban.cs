namespace MagazinEAPI.Models
{
	public abstract class BanAbstract
	{
		public int Id { get; set; }
		public int AdminId { get; set; }
		public int UserId { get; set; }
	}

	public class BanDTO: BanAbstract
	{
	}

	public class Ban : BanAbstract
	{
		public Admin Admin { get; set; } = null!;
		public User User { get; set; } = null!;

		public List<UnbanRequest> UnbanRequests { get; set; } = []; //one to many
	}
}
