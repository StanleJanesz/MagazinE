using MagazinEAPI.Contexts;
using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Users;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
using System.Security.Claims;


namespace MagazinEAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("subscriptions")]
    public class SubscriptionsController : Controller
    {
        private readonly RolesBasedContext _APIContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public SubscriptionsController(RolesBasedContext apiContext, UserManager<ApplicationUser> userManager) 
        {
            _APIContext = apiContext;
            _userManager = userManager;
        }

        [HttpGet("{userID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<SubscriptionDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SubscriptionDTO>> GetSubscription([FromRoute] int userID)
        {
            // User can get info about only their own subscription, unless they are admin

            if(!int.TryParse(_userManager.GetUserId(User), out int userId))
            {
                return BadRequest("User not found");
            }

            var isAdmin = User.IsInRole("Admin");
            var isReader = User.IsInRole("Reader");
          
            if (userId != userID && !isAdmin) 
            {
                return Unauthorized("This action is admin only!");
            }

            var subInfo = await _APIContext.Subscriptions.FirstOrDefaultAsync(sub => sub.UserId == userID);
            
            if (subInfo == null) 
            {
                return BadRequest("Subscription not found");
            }

            return Ok(subInfo.ToDTO());
        }

        [HttpPut("{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PutSubscription([FromRoute] int userID, [FromBody] SubscriptionDTO subscriptionDTO)
        {
            if (!int.TryParse(_userManager.GetUserId(User), out int userId))
            {
                return BadRequest("Invalid user");
            }
        
            var isAdmin = User.IsInRole("Admin");
            var isReader = User.IsInRole("Reader");

            if (userId != userID && !isAdmin)
            {
                return Unauthorized("You cannot modify another user's subscription.");
            }
            
            if (!isReader)
            {
                return BadRequest("Non-reader cannot subscribe!");
            }

            if (await _APIContext.Subscriptions.AnyAsync(s => s.UserId == userId))
            {
                return BadRequest("Subscription already exists. Use UpdateSubscription to update.");
            }

            try
            {   
                var subscription = new Subscription
                {
                    UserId = userID,
                    StartDate = subscriptionDTO.StartDate,
                    EndDate = subscriptionDTO.EndDate,
                    State = subscriptionDTO.State
                };
                
                await _APIContext.Subscriptions.AddAsync(subscription);
                await _APIContext.SaveChangesAsync();
                return Ok(subscription.ToDTO());
            }
            catch (Exception ex) 
            {
                return BadRequest($"Editing failed {ex.Message}");
            }
        }

        [HttpDelete("{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSubscription([FromRoute] int userID)
        {
            if (!int.TryParse(_userManager.GetUserId(User), out int userId))
            {
                return BadRequest("Invalid user");
            }

            var isAdmin = User.IsInRole("Admin");

            if (userId != userID && !isAdmin)
            {
                return Unauthorized("You cannot delete another user's subscription.");
            }

            var subscription = await _APIContext.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userID);

            if (subscription == null)
            {
                return NotFound("Subscription not found");
            }

            try
            {
                _APIContext.Subscriptions.Remove(subscription);
                await _APIContext.SaveChangesAsync();

                return Ok("Subscription successfully deleted");
            }
            catch (Exception ex) 
            { 
                return BadRequest($"Deleting subscription failed: {ex.Message}"); 
            }
        }


        [HttpPatch("{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSubscription([FromRoute] int userID, [FromBody] SubscriptionDTO subscriptionDTO)
        {
            var subscription = await _APIContext.Subscriptions.FirstOrDefaultAsync(sub => sub.UserId == userID);
            if (subscription == null)
            {
                return NotFound("No subscription exists");
            }

            subscription.StartDate = subscriptionDTO.StartDate;
            subscription.EndDate = subscriptionDTO.EndDate;
            subscription.State = subscriptionDTO.State;

            try
            {
                _APIContext.Subscriptions.Update(subscription);
                await _APIContext.SaveChangesAsync();
                return Ok(subscription.ToDTO());
            }
            catch (Exception ex)
            {
                return BadRequest($"Updating info failed {ex.Message}");
            }
        }

        private bool UserCanAccess(int targetUserId, out IActionResult? failureResult)
        {
            failureResult = null;
            if (!int.TryParse(_userManager.GetUserId(User), out var currentUserId))
                failureResult = BadRequest("Invalid user");
            else if (currentUserId != targetUserId && !User.IsInRole("Admin"))
                failureResult = Forbid();
            return failureResult == null;
        }

    }
}
