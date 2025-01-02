using E_Commerce.Models;

namespace E_Commerce.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductItem>> SearchProductsAsync(string keyword);
        Task AddToCartAsync(string userId, int productItemId, int quantity);
        Task RemoveFromCartAsync(string userId, int productItemId);
        Task<List<CartItem>> GetCartItemsAsync(string userId);
        Task AddToWishlistAsync(string userId, int productItemId);
        Task RemoveFromWishlistAsync(string userId, int productItemId);
        Task<List<WishListItems>> GetUserWishlistAsync(string userId);


    }
}
