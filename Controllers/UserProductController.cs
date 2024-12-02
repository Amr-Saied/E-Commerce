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
    public class UserProductController : ControllerBase
    {
        private readonly IProductService _ProductService;

        public UserProductController(IProductService productService)
        {
            _ProductService = productService;
        }

        [AllowAnonymous]
        [HttpGet("SearchProducts")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            try
            {
                var productItems = await _ProductService.SearchProductsAsync(keyword);

                if (productItems == null || !productItems.Any())
                {
                    return NotFound("No products found matching the given keyword.");
                }

                return Ok(productItems);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
