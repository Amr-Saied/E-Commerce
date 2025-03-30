using E_Commerce.Context;
using E_Commerce.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;
namespace E_Commerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ECommerceDbContext _context;
        public PaymentService(ECommerceDbContext context)
        {
            _context = context;
        }

        private async Task<decimal> GetTotalPriceBySystemIdAsync(string systemId)
        {
            var order = await _context.Orders
                .Where(o => o.UserId == systemId)
                .OrderByDescending(o => o.OrderDate) // Get the latest order
                .FirstOrDefaultAsync();

            return order?.OrderTotal ?? 0;
        }
        //hardcoded secret to be added to user_secrets next push
        private const string EndpointSecret = "whsec_2e8b5b94bfb28a921416d72edac2d0924a92b92269f431ea0c4ff24ff2896b30";
        public async Task<string> CreatePaymentIntentAsync(string email, string paymentMethodId, string userId, string name)
        {
            var user = _context.Users.Find(userId);
            var service = new PaymentIntentService();
            CreateStripeCustomerAsync(name, email, userId);

            var totalPrice = await GetTotalPriceBySystemIdAsync(userId);
            if (totalPrice <= 0)
                throw new ArgumentException("Invalid total price for order");

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)totalPrice,
                Currency = "usd",
                PaymentMethod = paymentMethodId,
                Confirm = true,
                ReceiptEmail = email,
                Metadata = new Dictionary<string, string>
                    {
                        { "ID", userId }
                    }
            };

            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent.Id;
        }
        //systemId = userId
        public async Task<bool> CreateStripeCustomerAsync(string name, string email, string systemId)
        {
            try
            {
                var options = new CustomerCreateOptions
                {
                    Email = email,
                    Name = name,
                    Metadata = new Dictionary<string, string>
                    {
                        { "ID", systemId },
                    }
                };

                var service = new CustomerService();
                await service.CreateAsync(options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HandleWebhookAsync(string json, string signatureHeader)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, EndpointSecret);

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent == null)
                        return false;


                    if (!paymentIntent.Metadata.TryGetValue("ID", out var systemId))
                        return false;


                    var order = await _context.Orders
                        .Include(o => o.OrderLines)
                        .FirstOrDefaultAsync(o => o.UserId == systemId);

                    if (order == null || !order.OrderLines.Any())
                        return false;

                    // Check stock availability
                    bool allItemsAvailable = true;
                    foreach (var orderLine in order.OrderLines)
                    {
                        var productItem = await _context.ProductItems.FindAsync(orderLine.ProductItemId);
                        if (productItem == null || productItem.QtyInStock < orderLine.Qty)
                        {
                            allItemsAvailable = false;
                            break;
                        }
                    }

                    if (allItemsAvailable)
                    {
                        foreach (var orderLine in order.OrderLines)
                        {
                            var productItem = await _context.ProductItems.FindAsync(orderLine.ProductItemId);
                            if (productItem != null)
                            {
                                //validation to be added in case user tries to buy more than in stock
                                productItem.QtyInStock -= orderLine.Qty;
                            }
                        }


                        var cart = await _context.Carts
                            .FirstOrDefaultAsync(c => c.UserId == order.UserId);

                        if (cart != null)
                        {

                            var cartItems = await _context.CartItems
                                .Where(ci => ci.CartId == cart.CartId && order.OrderLines.Any(o => o.ProductItemId == ci.ProductItemId))
                                .ToListAsync();

                            _context.CartItems.RemoveRange(cartItems);
                        }

                        order.OrderStatusId = 2;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        var refundOptions = new RefundCreateOptions
                        {
                            PaymentIntent = paymentIntent.Id,
                            Amount = paymentIntent.Amount,
                        };

                        var refundService = new RefundService();
                        await refundService.CreateAsync(refundOptions);

                        return false;
                    }
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentCanceled ||
                         stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                    Console.WriteLine($"⚠️ Payment failed or canceled: {stripeEvent.Type}");
                    return false;
                }

                Console.WriteLine($"⚠️ Unhandled event type: {stripeEvent.Type}");
                return false;
            }
            catch (StripeException e)
            {
                Console.WriteLine($"❌ Stripe error: {e.Message}");
                return false;
            }
        }



    }
}