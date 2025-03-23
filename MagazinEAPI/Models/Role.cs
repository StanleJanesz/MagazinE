using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models
{
	public abstract class Role : RoleBase
	{
		public string ApplicationUserId { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
	}
}
