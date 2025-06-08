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
using Stripe;
using Stripe.Checkout;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Web;

namespace MagazinEAPI.Controllers
{
    [Authorize]
    [Authorize(AuthenticationSchemes = "Bearer")]
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetSubscriptions([FromRoute] string userID)
        {
            // User can get info about only their own subscription, unless they are admin
            var (canAccess, targetUser, failure) = await ValidateUserAsync(userID);

            if (!canAccess)
            {
                return failure!;
            }

            var isReader = await _userManager.IsInRoleAsync(targetUser!, "Reader");

            if (!isReader)
            {
                return BadRequest("User is not a reader. Modify role first");
            }

            try
            {
                var subInfo = await _APIContext.Subscriptions
                    .Where(sub => sub.User.ApplicationUserId == userID)
                    .Select(sub => sub.ToDTO())
                    .ToListAsync();

                return Ok(subInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostSubscription([FromRoute] string userID)
        {
            //var (canAccess, targetUser, failure) = await ValidateUserAsync(userID);

            //if (!canAccess)
            //{
            //    return failure!;
            //}

            //var isReader = await _userManager.IsInRoleAsync(targetUser!, "Reader");

            //if (!isReader)
            //{
            //    return BadRequest("User is not a reader. Modify role first.");
            //}

            //try
            //{
            //    var reader = _APIContext.Readers
            //        .Where(r => r.ApplicationUserId == userID)
            //        .First();

            //    if (reader == null)
            //    {
            //        return StatusCode(500, "Could not find the reader");
            //    }

            //    var subscription = new Subscription
            //    {
            //        UserId = reader.Id,
            //        StartDate = subscriptionDTO.StartDate,
            //        EndDate = subscriptionDTO.EndDate,
            //        State = subscriptionDTO.State,
            //        User = reader
            //    };

            //    await _APIContext.Subscriptions.AddAsync(subscription);
            //    await _APIContext.SaveChangesAsync();

            //    return Ok(subscription.ToDTO());
            //}
            //catch (Exception ex) 
            //{
            //    return BadRequest($"Editing subscription failed {ex.Message}");
            //}
            try
            {
                // Get the email from the current user's claims
                var email = this.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return this.BadRequest("User email not found in claims");
                }

                // Optional: Add validation to ensure the userID matches the current user
                // This depends on your security requirements

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "subscription",
                    CustomerEmail = email,
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = "price_1R6VAjQTT0aReMtnWGhWBYNT",
                            Quantity = 1,
                        },
                    },
                    SuccessUrl = "http://localhost:5173/subscription-success",
                    CancelUrl = "http://localhost:5173/subscription-cancel",
                    Locale = "en",

                    // Optional: Add metadata to track the user
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userID },
                    },
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new { sessionId = session.Id });
            }
            catch (StripeException stripeEx)
            {
                // Log the full exception details
                Console.WriteLine($"Stripe error: {stripeEx.Message}");
                return StatusCode(500, new { error = "Payment processing error", details = stripeEx.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"General error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpPatch("{userID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateSubscription([FromRoute] string userID, [FromBody] SubscriptionDTO subscriptionDTO)
        {
            var (canAccess, targetUser, failure) = await ValidateUserAsync(userID);
            if (!canAccess)
            {
                return failure!;
            }

            var isReader = await _userManager.IsInRoleAsync(targetUser!, "Reader");

            if (!isReader)
            {
                return BadRequest("User is not a reader. Modify role first.");
            }

            try
            {
                var subscriptions = _APIContext.Subscriptions
                    .Where(sub => sub.User.ApplicationUserId == userID);

                var subscription = subscriptions.First(sub => sub.Id == subscriptionDTO.Id);

                if (subscription == null)
                {
                    return NotFound("No subscription exists");
                }

                subscription.StartDate = subscriptionDTO.StartDate;
                subscription.EndDate = subscriptionDTO.EndDate;
                subscription.State = subscriptionDTO.State;

                _APIContext.Subscriptions.Update(subscription);
                await _APIContext.SaveChangesAsync();

                return Ok(subscription.ToDTO());
            }
            catch (Exception ex)
            {
                return BadRequest($"Updating info failed {ex.Message}");
            }
        }

        private async Task<(bool CanAccess, ApplicationUser? TargetUser, IActionResult? FailureResult)> ValidateUserAsync(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return (false, null, StatusCode(500, "Authenticated user not found."));
            }

            var targetUser = await _userManager.FindByIdAsync(userId);

            if (targetUser == null)
            {
                return (false, null, BadRequest("User does not exist."));
            }

            var isAdmin = User.IsInRole("Admin");

            if (currentUserId != userId && !isAdmin)
            {
                return (false, targetUser, Forbid());
            }

            return (true, targetUser, null);
        }
    }
}