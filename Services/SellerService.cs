using E_Commerce.Context;
using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class SellerService : ISellerService
    {
        private readonly ECommerceDbContext _context;
        private readonly INotificationService _notificationService;
        public SellerService(ECommerceDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<ProductItem> AddProductItemAsync(ProductItem productItem)
        {
            await _context.ProductItems.AddAsync(productItem);
            await _context.SaveChangesAsync();
            return productItem;
        }

        public async Task<ProductConfiguration> AddProductConfigurationAsync(ProductConfiguration productConfiguration)
        {
            await _context.ProductConfigurations.AddAsync(productConfiguration);
            await _context.SaveChangesAsync();
            return productConfiguration;
        }

        public async Task<bool> CheckCategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }

        public async Task<bool> CheckVariationExistsAsync(int variationId)
        {
            return await _context.Variations.AnyAsync(v => v.Id == variationId);
        }

        public async Task<IEnumerable<ProductItem>> GetProductItemsBySellerAsync(string sellerId)
        {
            return await _context.ProductItems
                .Where(pi => pi.SellerId == sellerId)
                .Include(pi => pi.Product)
                .ToListAsync();
        }
        public async Task<ProductItem> DeleteProductItemAsync(int productItemId, string sellerId)
        {
            var productItem = await _context.ProductItems
                .Where(pi => pi.Id == productItemId && pi.SellerId == sellerId)
                .FirstOrDefaultAsync();

            if (productItem != null)
            {
                _context.ProductItems.Remove(productItem);
                await _context.SaveChangesAsync();
            }

            return productItem;

        }
        public async Task<ProductItem> EditProductItemAsync(EditProductDTO productItem, string sellerId, int productID)
        {
            var existingProductItem = await _context.ProductItems
                .Where(pi => pi.Id == productID && pi.SellerId == sellerId)
                .FirstOrDefaultAsync();

            if (existingProductItem != null)
            {
                existingProductItem.Price = productItem.Price;
                existingProductItem.Description = productItem.Description;
                if(existingProductItem.QtyInStock == 0 && productItem.QtyInStock != 0)
                    _ = Task.Run(() => _notificationService.NotifyUserWishListAsync(existingProductItem.Id));
                existingProductItem.QtyInStock = productItem.QtyInStock;
                existingProductItem.ProductImage = productItem.ProductImage;
                await _context.SaveChangesAsync();
                return existingProductItem;
            }

            return null;
        }
    }
}
