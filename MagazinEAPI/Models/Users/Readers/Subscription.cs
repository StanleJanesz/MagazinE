using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;

namespace MagazinEAPI.Models.Users.Readers
{
    public class Subscription : SubscriptionAbstract, DTOable<SubscriptionDTO>
    {
        public User User { get; set; } = null!; //has 1 user
        public SubscriptionDTO ToDTO() 
        {
            return new SubscriptionDTO
            {
                Id = this.Id,
                UserId = this.UserId,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                State = this.State,
            }; 
        }
    }
}
