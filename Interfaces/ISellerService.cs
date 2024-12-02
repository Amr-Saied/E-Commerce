﻿using E_Commerce.DTO;
using E_Commerce.Models;
using System.Threading.Tasks;

namespace E_Commerce.Interfaces
{
    public interface ISellerService
    {
        Task<Product> AddProductAsync(Product product);

        Task<ProductItem> AddProductItemAsync(ProductItem productItem);

        Task<ProductConfiguration> AddProductConfigurationAsync(ProductConfiguration productConfiguration);

        Task<bool> CheckCategoryExistsAsync(int categoryId);

        Task<bool> CheckVariationExistsAsync(int variationId);

        Task<IEnumerable<ProductItem>> GetProductItemsBySellerAsync(string sellerId);

        Task<ProductItem> DeleteProductItemAsync(int productItemId, string sellerId);

        Task<ProductItem> EditProductItemAsync(EditProductDTO productItem, string sellerId, int productID);


    }
}
