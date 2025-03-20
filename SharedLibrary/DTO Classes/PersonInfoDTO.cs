using SharedLibrary.Base_Classes___Database;
namespace SharedLibrary.DTO_Classes
{

	public class PersonInfoDTO
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Login { get; set; }
		public UserState State { get; set; }
	}

}
