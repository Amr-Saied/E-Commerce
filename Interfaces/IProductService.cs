using E_Commerce.Models;

namespace E_Commerce.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductItem>> SearchProductsAsync(string keyword);
        public Task<bool> AddProductAsync(Product product);

    }
}
