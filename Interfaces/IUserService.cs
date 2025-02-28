using E_Commerce.Models;

namespace E_Commerce.Interfaces
{
    public interface IUserService
    {
        Task<ShippingAddress> AddShippingAddressAsync(string userId, ShippingAddress shippingAddress);
        Task<IEnumerable<ShippingAddress>> GetShippingAddressesByUserIdAsync(string userId);
    }
}
