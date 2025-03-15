using E_Commerce.DTO;
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


        public Task<bool> AddProductAsync(Product product);

        Task AddReviewAsync(Review review);

        public Task<Product?> GetProductByNormalizedNameAsync(string normalizedProductName);

        public Task<bool> AddProductItemAsync(ProductItem productItem);

        public Task<bool> UpdateProductItemAsync(ProductItem productItem);

        public Task<ProductItem> GetProductItemByIdAsync(int productItemId);

        public Task<IEnumerable<ProductItem>> GetProductItemsByCategoryAsync(int categoryId);

        public Task<IEnumerable<ProductItemDTO>> GetTopRatedProductsAsync();

    }
}
