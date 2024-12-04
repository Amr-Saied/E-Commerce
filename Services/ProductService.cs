using E_Commerce.Context;
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

    }
}
