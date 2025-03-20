using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models
{

	public class UnbanRequest : UnbanRequestAbstract
	{
		public Ban Ban { get; set; } = null!; //has 1 Ban (1 to many)
	}

}
