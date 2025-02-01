using E_Commerce.Context;
using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        public async Task<bool> AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ValidationException("Product name is required.");
            }

            if (product.CategoryId <= 0)
            {
                throw new ValidationException("Invalid category ID.");
            }

            if (product.ProductItems == null || product.ProductItems.Count == 0)
            {
                throw new ValidationException("Product must have at least one product item.");
            }

            foreach (var item in product.ProductItems)
            {
                if (string.IsNullOrEmpty(item.SKU))
                {
                    throw new ValidationException("SKU is required for each product item.");
                }

                if (item.Price <= 0)
                {
                    throw new ValidationException("Price must be greater than zero.");
                }

                if (item.QtyInStock < 0)
                {
                    throw new ValidationException("Quantity in stock cannot be negative.");
                }
            }

            try
            {
                await _context.Products.AddAsync(product);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
        public async Task<Product?> GetProductByNormalizedNameAsync(string normalizedProductName)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.ProductItems) 
                    .FirstOrDefaultAsync(p => p.Name == normalizedProductName);
            }
            catch (Exception ex)
            {
                return null; 
            }
        }

        public async Task<bool> AddProductItemAsync(ProductItem productItem)
        {
            try
            {
                _context.ProductItems.Add(productItem);

                var result = await _context.SaveChangesAsync();

                return result > 0; 
            }
            catch (Exception ex)
            {
                return false; 
            }
        }

        public async Task<bool> UpdateProductItemAsync(ProductItem productItem)
        {
            try
            {
                _context.ProductItems.Update(productItem);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ProductItem?> GetProductItemByIdAsync(int productItemId)
        {
            return await _context.ProductItems
                .Include(pi => pi.Product) // Include related product details
                .Include(pi => pi.Seller) // Optionally include the seller
                .FirstOrDefaultAsync(pi => pi.Id == productItemId);
        }

        public async Task<IEnumerable<ProductItemDTO>> GetTopRatedProductsAsync()
        {
            var items = await _context.ProductItems
                .Include(p => p.OrderLines)
                .ThenInclude(ol => ol.Reviews)
                .Where(p => p.OrderLines.Any(ol => ol.Reviews.Any()))
                .Select(p => new ProductItemDTO
                {
                    Id = p.Id,
                    Name = p.Product.Name,
                    Price = p.Price,
                    Description = p.Description,
                    ProductImage = p.ProductImage,
                    AverageRating = p.OrderLines
                                      .SelectMany(ol => ol.Reviews)
                                      .Average(r => (double?)r.RatingValue) ?? 0
                })
                .OrderByDescending(p => p.AverageRating)
                .ToListAsync();

            return items;
        }

        public async Task<IEnumerable<ProductItem>> GetProductItemsByCategoryAsync(int categoryId)
        {

            var items = await _context.ProductItems
                                      .Where(pi => pi.Product.CategoryId == categoryId)
                                      .OrderBy(pi => pi.Id)
                                      .ToListAsync();

            return items;
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

    }
}
