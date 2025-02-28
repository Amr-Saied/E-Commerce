using E_Commerce.Context;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class UserService : IUserService
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
    }
}
