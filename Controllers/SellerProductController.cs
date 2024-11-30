using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class SellerProductController : ControllerBase
    {
        private readonly ISellerService _SellerService;
        private readonly ICategoryService _CategoryService;
        private readonly IVariationService _VariationService;

        public SellerProductController(ISellerService sellerService, ICategoryService categoryService, IVariationService variationService)
        {
            _SellerService = sellerService;
            _CategoryService = categoryService;
            _VariationService = variationService;
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _CategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("GetVariations/{categoryId}")]
        public async Task<IActionResult> GetVariations(int categoryId)
        {
            var variations = await _VariationService.GetVariationsByCategoryAsync(categoryId);
            return Ok(variations);
        }

        [HttpGet("GetVariationOptions/{variationId}")]
        public async Task<IActionResult> GetVariationOptions(int variationId)
        {
            var variationOptions = await _VariationService.GetOptionsByVariationAsync(variationId);
            return Ok(variationOptions);
        }

        [Authorize(Roles = "Seller, Admin")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null || productDTO.CategoryId == 0)
            {
                return BadRequest("Invalid product data");
            }

            var product = new Product
            {
                Name = productDTO.Name,
                CategoryId = productDTO.CategoryId
            };

            var savedProduct = await _SellerService.AddProductAsync(product);

            foreach (var variationDTO in productDTO.Variations)
            {
                var productItem = new ProductItem
                {
                    ProductId = savedProduct.Id,
                    SKU = GenerateSKU(savedProduct.Id, variationDTO.VariationId), 
                    Price = variationDTO.Price,
                    Description = productDTO.Description,
                    SellerId = "sellerId",
                    QtyInStock = variationDTO.QtyInStock, 
                    ProductImage = variationDTO.ProductImage 
                };

                await _SellerService.AddProductItemAsync(productItem);

                foreach (var optionId in variationDTO.VariationOptionIds)
                {
                    var productConfiguration = new ProductConfiguration
                    {
                        ProductItem = productItem,
                        VariationOptionId = optionId
                    };

                    await _SellerService.AddProductConfigurationAsync(productConfiguration);
                }
            }

            return Ok(savedProduct);
        }

        private string GenerateSKU(int productId, int variationId)
        {
            return $"SKU-{productId}-{variationId}";
        }
    }
}
