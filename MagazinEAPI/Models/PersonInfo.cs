using Microsoft.AspNetCore.Identity;
namespace MagazinEAPI.Models
{

	public class PersonInfo
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Login { get; set; }
		public UserState State { get; set; }
	}

}
