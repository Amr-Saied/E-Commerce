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
        private readonly IPaginationService _PaginationService;
        private readonly IUserService _UserService;

        public UserProductController(IProductService productService, IPaginationService paginationService, IUserService userService)
        {
            _ProductService = productService;
            _PaginationService = paginationService;
            _UserService = userService;
        }

        [AllowAnonymous]
        [HttpGet("SearchProducts")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword, int pageNumber = 1, int pageSize = 10)
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

                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page number and page size must be greater than zero.");
                }

                var paginatedResult = await _PaginationService.GetPaginatedResultAsync(productItems, pageNumber, pageSize);

                return Ok(paginatedResult);
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


        [AllowAnonymous]
        [HttpGet("TopRatedProducts")]
        public async Task<IActionResult> GetTopRatedProducts(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page number and page size must be greater than zero.");
                }

                var topRatedItems = await _ProductService.GetTopRatedProductsAsync();

                if (topRatedItems == null || !topRatedItems.Any())
                {
                    return NotFound("No top-rated products found.");
                }

                var paginatedResult = _PaginationService.GetPaginatedResultAsync(topRatedItems, pageNumber, pageSize);

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetProductItemsByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductItemsByCategory(int categoryId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page number and page size must be greater than zero.");
                }

                var productItems = await _ProductService.GetProductItemsByCategoryAsync(categoryId);

                if (productItems == null || !productItems.Any())
                {
                    return NotFound("No products found in the specified category.");
                }

                var paginatedResult = await _PaginationService.GetPaginatedResultAsync(productItems, pageNumber, pageSize);

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCart(int productItemId, int quantity)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _ProductService.AddToCartAsync(userId, productItemId, quantity);
                return Ok("Item added to cart successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "User")]
        [HttpPost("removeFromCart")]
        public async Task<IActionResult> RemoveFromCart(int productItemId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _ProductService.RemoveFromCartAsync(userId, productItemId);
                return Ok("Item removed from cart successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "User")]
        [HttpGet("user-cart-items")]
        public async Task<IActionResult> GetUserCartItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { Error = "User is not authenticated." });
            }

            var cartItems = await _ProductService.GetCartItemsAsync(userId);

            return Ok(cartItems);
        }
        [Authorize(Roles = "User")]
        [HttpPost("add_wishlsit")]
        public async Task<IActionResult> AddToWishlist([FromBody] int productItemId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _ProductService.AddToWishlistAsync(userId, productItemId);
                return Ok(new { Message = "Item added to wishlist successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpDelete("remove_wishlist")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] int productItemId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _ProductService.RemoveFromWishlistAsync(userId, productItemId);
                return Ok(new { Message = "Item removed from wishlist successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpGet("get_wishlist")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserWishlist()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlist = await _ProductService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }
        [Authorize(Roles = "User")]
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var order = await _UserService.CheckoutAsync(userId);
                return Ok(new { message = "Checkout successful", order });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
