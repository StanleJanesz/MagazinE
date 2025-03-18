namespace MagazinEAPI.Models
{
	public abstract class RoleBase
	{
		public int Id { get; set; }
	}


	public abstract class RoleDTO : RoleBase
	{
		public int PersonInfoId { get; set; }
	}

	public abstract class Role : RoleBase
	{
		public string ApplicationUserId { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
	}
}
