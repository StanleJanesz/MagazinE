using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models;
public class RegisterRequest : RegisterRequestAbstract
{
	public bool IsSuccesfull { get; set; }
	public DateTime RegisterDateTime { get; set; }
}
