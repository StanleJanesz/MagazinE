using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace MagazinEAPI.Controllers
{
    public class StripeController : Controller
    {
        private readonly IConfiguration _configuration;

        public StripeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("stripe/webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            string endpointSecret = _configuration["Stripe:EndpointSecret"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    this.Request.Headers["Stripe-Signature"],
                    endpointSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    string userId = session.Metadata["user_id"];
                    string stripeCustomerId = session.CustomerId;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return BadRequest();
            }
        }
    }
}
