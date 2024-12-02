using E_Commerce.Context;
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
    }
}
