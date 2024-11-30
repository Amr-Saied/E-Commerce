using E_Commerce.Context;
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

        public SellerService(ECommerceDbContext context)
        {
            _context = context;
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
    }
}
