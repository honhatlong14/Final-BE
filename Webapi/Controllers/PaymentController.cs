using Data.Entities;
using Data.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.IServices;
using Stripe;
using Stripe.Checkout;
using StringEnum = Common.Constants.StringEnum;

namespace Webapi.Controllers;

[Route("create-payment-intent")]
[ApiController]
public class PaymentIntentApiController : Controller
{
    private const string WEB_HOOK_SECRET = "whsec_aedaa4162f609728da15ab31242fa6f1c8889290c706386e65a77801eb6f8250";
    private readonly ILogger<PaymentIntentApiController> _logger;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IRepository<Order> _orderRepo;


    public PaymentIntentApiController(ILogger<PaymentIntentApiController> logger, ICartService cartService,
        IOrderService orderService, IRepository<Order> orderRepo)
    {
        _logger = logger;
        _cartService = cartService;
        _orderService = orderService;
        _orderRepo = orderRepo;
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> Create(string userId)
    {
        var paymentIntentService = new PaymentIntentService();

        var options = new PaymentIntentCreateOptions
        {
            Amount = await CalculateOrderAmount(userId),
            Currency = "usd",
            // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
        };

        var paymentIntent = await paymentIntentService.CreateAsync(options);

        if (paymentIntent.Status == StringEnum.PAYMENT_REQUIRED_METHOD)
        {
            var order = await _orderService.CreateOrderOrderDetail(userId, paymentIntent.Id,
                StringEnum.PaymentMethod.Card);
        }

        return Json(new { clientSecret = paymentIntent.ClientSecret });
    }

    [HttpPost("webHook")]
    public async Task<ActionResult> WebHookHandler()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json,
                signatureHeader, WEB_HOOK_SECRET);

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var order = await _orderRepo.GetFirstOrDefaultAsync(o =>
                    o.PaymentId == paymentIntent.Id && o.PaymentMethod == StringEnum.PaymentMethod.Card);
                
                await _cartService.RemoveListCartItem(order.UserId);
                order.PaymentStatus = StringEnum.PAYMENT_SUCCESS;
                _orderRepo.Update(order);
                await _orderRepo.SaveChangesAsync();
            }
            else if (stripeEvent.Type == Events.PaymentMethodAttached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                // Then define and call a method to handle the successful attachment of a PaymentMethod.
                // handlePaymentMethodAttached(paymentMethod);
            }
            else
            {
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }

            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }


    private async Task<int> CalculateOrderAmount(string userId)
    {
        var totalOrderAmount = 0;
        var cartItems = await _cartService.GetAllCartSelected(userId);
        foreach (var item in cartItems)
        {
            totalOrderAmount += item.Total * 100;
        }

        return totalOrderAmount;
    }

    public class Item
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("Amount")] public string Amount { get; set; }
    }

    public class PaymentIntentCreateRequest
    {
        [JsonProperty("items")] public Item[] Items { get; set; }
    }
}