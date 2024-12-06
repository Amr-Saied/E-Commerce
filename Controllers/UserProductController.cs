using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        [Authorize(Roles = "Admin, User")]
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDTO)
        {

            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (reviewDTO == null)
            {
                return BadRequest("Review data is required.");
            }

            if (reviewDTO.RatingValue < 1 || reviewDTO.RatingValue > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            var productItem = await _ProductService.GetProductItemByIdAsync(reviewDTO.ProductItemId);
            if (productItem == null)
            {
                return NotFound("Product not found.");
            }
            var review = new Review
            {
                UserId = UserId,
                RatingValue = reviewDTO.RatingValue,
                Comment = reviewDTO.Comment,
                OrderLineId = reviewDTO.OrderLineId,
                OrderLine = reviewDTO.OrderLine,
            };

            try
            {
                // Add the review through the review service
                await _ProductService.AddReviewAsync(review);

                return Ok("Review added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding review: {ex.Message}");
            }
        }

    }
}
