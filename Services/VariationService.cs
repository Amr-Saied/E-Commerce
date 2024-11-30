using E_Commerce.Context;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Google;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class VariationService : IVariationService
    {
        private readonly ECommerceDbContext _dbContext;

        public VariationService(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Variation>> GetVariationsByCategoryAsync(int categoryId)
        {
            return await _dbContext.Variations
                .Where(v => v.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VariationOption>> GetOptionsByVariationAsync(int variationId)
        {
            return await _dbContext.VariationOptions
                .Where(vo => vo.VariationId == variationId)
                .ToListAsync();
        }

    }
}
