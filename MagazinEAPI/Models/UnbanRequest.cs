namespace MagazinEAPI.Models
{
	public abstract class UnbanRequestAbstract
	{
		public int Id { get; set; }
		public string Reason { get; set; }
		public int BanId { get; set; }
		public UnbanRequestState State { get; set; }
	}
	public class UnbanRequestDTO : UnbanRequestAbstract
	{
		
	}

	public class UnbanRequest : UnbanRequestAbstract
	{
		public Ban Ban { get; set; } = null!; //has 1 Ban (1 to many)
	}

}
