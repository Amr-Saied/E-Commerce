using E_Commerce.Context;
using E_Commerce.DTO;
using E_Commerce.Interfaces;
using Google;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ECommerceDbContext _context;

        public CategoryService(ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.CategoryId,
                    Name = c.CategoryName,
                    ParentCategoryId = c.ParentCategoryId,
                })
                .ToListAsync();

            return categories;
        }
    }
}
