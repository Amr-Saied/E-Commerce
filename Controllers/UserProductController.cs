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
    }
}
