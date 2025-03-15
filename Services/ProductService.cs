using E_Commerce.Context;
using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly ECommerceDbContext _context;

        public ProductService(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductItem>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Keyword cannot be null or empty.", nameof(keyword));
            }

            return await _context.ProductItems
                .Include(pi => pi.Product)
                .Where(pi => pi.Product.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                             pi.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                             pi.Product.Category.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                             pi.Product.Category.ParentCategory.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task AddToCartAsync(string userId, int productItemId, int quantity)
        {
            var user = await _context.Users
                .Include(u => u.cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new Exception("User not found");

            var product = await _context.ProductItems.FindAsync(productItemId);
            if (product == null) throw new Exception("Product not found");

            var cart = user.cart;

            // If the cart does not exist, create a new one
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    User = user,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                user.cart = cart;
            }

            // Check if the product already exists in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductItemId == productItemId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductItemId = productItemId,
                    ProductItem = product,
                    Qty = quantity,
                    CartId = cart.CartId,
                    Cart = cart
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Qty += quantity;
            }


            foreach (var item in cart.CartItems)
            {
                item.CartId = cart.CartId;
                item.Cart = cart;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productItemId)
        {
            // Retrieve the user and associated cart
            var user = await _context.Users
                .Include(u => u.cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new Exception("User not found");

            var cart = user.cart;

            if (cart == null) throw new Exception("Cart not found");

            // Find the item in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductItemId == productItemId);

            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);

                // Remove cart if it is empty
                if (!cart.CartItems.Any())
                {
                    _context.Carts.Remove(cart);
                    user.cart = null;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item not found in cart");
            }
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string userId)
        {
            var cart = await _context.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.ProductItem)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                throw new KeyNotFoundException("Cart not found for the specified user.");
            }

            return cart.CartItems.ToList();

        }

        public async Task AddToWishlistAsync(string userId, int productItemId)
        {
            var productItem = await _context.ProductItems
             .FirstOrDefaultAsync(pi => pi.Id == productItemId);

            if (productItem == null)
            {
                throw new KeyNotFoundException("The specified product item does not exist.");
            }

            if (productItem.QtyInStock > 0)
            {
                throw new InvalidOperationException("This product item is currently in stock and cannot be added to the wishlist.");
            }

            bool exists = await _context.WishlistItems
                .AnyAsync(wi => wi.UserId == userId && wi.ProductItemId == productItemId);

            if (exists)
            {
                throw new InvalidOperationException("This item is already in the wishlist.");
            }

            var wishlistItem = new WishListItems
            {
                UserId = userId,
                ProductItemId = productItemId,
                AddedDate = DateTime.UtcNow
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(string userId, int productItemId)
        {
            var wishlistItem = await _context.WishlistItems
           .FirstOrDefaultAsync(wi => wi.UserId == userId && wi.ProductItemId == productItemId);

            if (wishlistItem == null)
            {
                throw new KeyNotFoundException("The item is not in the wishlist.");
            }

            // Remove the wishlist item
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WishListItems>> GetUserWishlistAsync(string userId)
        {
            return await _context.WishlistItems
            .Where(wi => wi.UserId == userId)
            .Include(wi => wi.ProductItem)
                .ThenInclude(pi => pi.Product) 
            .ToListAsync();
        }

        public Task<bool> AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task AddReviewAsync(Review review)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetProductByNormalizedNameAsync(string normalizedProductName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddProductItemAsync(ProductItem productItem)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductItemAsync(ProductItem productItem)
        {
            throw new NotImplementedException();
        }

        public Task<ProductItem> GetProductItemByIdAsync(int productItemId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductItem>> GetProductItemsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductItemDTO>> GetTopRatedProductsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
