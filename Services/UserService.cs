using E_Commerce.Context;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Services
{
    public class UserService: IUserService
    {
        private readonly ECommerceDbContext _context;

        public UserService(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<ShippingAddress> AddShippingAddressAsync(string userId, ShippingAddress shippingAddress)
        {
            shippingAddress.UserId = userId; 
            _context.ShippingAddresses.Add(shippingAddress);
            await _context.SaveChangesAsync();
            return shippingAddress;
        }

        public async Task<IEnumerable<ShippingAddress>> GetShippingAddressesByUserIdAsync(string userId)
        {
            return await _context.ShippingAddresses
                                 .Where(sa => sa.UserId == userId)
                                 .ToListAsync();
        }
        public async Task<Order> CheckoutAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems.Count == 0)
            {
                throw new Exception("Cart not found for the user.");
            }
            var shippingAddress = await _context.ShippingAddresses
               .Where(sa => sa.UserId == userId)
               .OrderByDescending(sa => sa.IsDefault) 
               .FirstOrDefaultAsync();

            decimal totalPrice = cart.CartItems.Sum(ci => ci.ProductItem.Price * ci.Qty);
            if (shippingAddress == null)
            {
                throw new Exception("No shipping address found for the user.");
            }
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderTotal = totalPrice,
                OrderStatusId = 1,
                ShippingAddressId = shippingAddress.UserAddressId,
                OrderLines = cart.CartItems.Select(ci => new OrderLine
                {
                    ProductItemId = ci.ProductItemId,
                    Qty = ci.Qty,
                    Price = ci.ProductItem.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
