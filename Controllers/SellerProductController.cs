using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        private readonly IProductService _ProductService;

        public SellerProductController(ISellerService sellerService, ICategoryService categoryService, 
            IVariationService variationService, IProductService productService)
        {
            _SellerService = sellerService;
            _CategoryService = categoryService;
            _VariationService = variationService;
            _ProductService = productService;
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

        [Authorize(Roles = "Seller, Admin")]
        [HttpGet("GetProductItems/{sellerId}")]
        public async Task<IActionResult> GetProductItems(string sellerId)
        {
            var realSellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (realSellerId == sellerId)
            {
                var productItems = await _SellerService.GetProductItemsBySellerAsync(sellerId);
                if (productItems == null || !productItems.Any())
                {
                    return NotFound("No product items found for this seller.");
                }

                return Ok(productItems);
            }
            else
            {
                return Forbid("Not Authorized");
            }

        }

        [Authorize(Roles = "Seller, Admin")]
        [HttpDelete("DeleteProductItem/{productItemId}")]
        public async Task<IActionResult> DeleteProductItem(int productItemId)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productItem = await _SellerService.DeleteProductItemAsync(productItemId, sellerId);

            if (productItem == null)
            {
                return NotFound("Product item not found or you do not have permission to delete this item.");
            }

            return Ok("Product item deleted successfully.");
        }

        [Authorize(Roles = "Seller, Admin")]
        [HttpPut("EditProductItem/{productItemId}")]
        public async Task<IActionResult> EditProductItem(int productItemId, [FromBody] EditProductDTO productItemDTO)
        {
            if (productItemDTO == null)
            {
                return BadRequest("Invalid product data.");
            }
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedProductItem = await _SellerService.EditProductItemAsync(productItemDTO, sellerId, productItemId);

            if (updatedProductItem == null)
            {
                return NotFound("Product item not found or you do not have permission to edit this item.");
            }

            return Ok(updatedProductItem);
        }

        private string GenerateSKU(int productId, int variationId)
        {
            return $"SKU-{productId}-{variationId}";
        }
    }
}
