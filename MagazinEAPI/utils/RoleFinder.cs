using MagazinEAPI.Models;

namespace MagazinEAPI.utils
{
    public class RoleFinder
    {
        public RoleFinder() { }

        // Metoda zakłada, że każdemu użytkownikowi przypisuje się jedną rolę. Przy innym założeniu, należy zmodyfikować metodę, tak aby zwracała wszystkie role.
        public string FindRole(ApplicationUser user)
        {            
            if(user.User != null)
            {
                return "User";
            }
            if (user.Journalist != null)
            {
                return "Journalist";
            }
            if (user.Editor != null)
            {
                return "Editor";
            }
            if(user.HeadEditor != null)
            {
                return "Head Editor";
            }
            if(user.Admin != null)
            {
                return "Admin";
            }
            throw new Exception("Żadna rola nie jest przypisana");
        }
    }
}
