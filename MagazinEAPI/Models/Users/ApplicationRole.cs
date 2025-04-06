using Microsoft.AspNetCore.Identity;


namespace MagazinEAPI.Models.Users
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }

       public ApplicationRole()
        {
            Description = string.Empty;
        }
    }
}
