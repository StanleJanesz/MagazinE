using SharedLibrary.Base_Classes___Database;
namespace MagazinEAPI.Models.Requests;
public class RegisterRequest : RegisterRequestAbstract
{
    public bool IsSuccesfull { get; set; }
    public DateTime RegisterDateTime { get; set; }
}
