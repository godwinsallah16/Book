using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStore.API.DTOs;
using BookStore.API.Services.Interfaces;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Get user's cart items
        /// </summary>
        /// <returns>List of cart items</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetUserCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var cartItems = await _cartService.GetUserCartAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user cart");
                return StatusCode(500, "An error occurred while fetching cart");
            }
        }

        /// <summary>
        /// Add book to cart
        /// </summary>
        /// <param name="addToCartDto">Book to add to cart</param>
        /// <returns>Added cart item</returns>
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto.BookId, addToCartDto.Quantity);
                if (cartItem == null)
                {
                    return BadRequest("Book not found or insufficient stock");
                }

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book to cart");
                return StatusCode(500, "An error occurred while adding to cart");
            }
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        /// <param name="bookId">Book ID</param>
        /// <param name="updateCartItemDto">Updated quantity</param>
        /// <returns>Updated cart item</returns>
        [HttpPut("{bookId}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(int bookId, [FromBody] UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var cartItem = await _cartService.UpdateCartItemAsync(userId, bookId, updateCartItemDto.Quantity);
                if (cartItem == null)
                {
                    return BadRequest("Cart item not found or insufficient stock");
                }

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart item");
                return StatusCode(500, "An error occurred while updating cart item");
            }
        }

        /// <summary>
        /// Remove book from cart
        /// </summary>
        /// <param name="bookId">Book ID to remove from cart</param>
        /// <returns>Success status</returns>
        [HttpDelete("{bookId}")]
        public async Task<ActionResult> RemoveFromCart(int bookId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _cartService.RemoveFromCartAsync(userId, bookId);
                if (!result)
                {
                    return NotFound("Book not found in cart");
                }

                return Ok(new { message = "Book removed from cart" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing book from cart");
                return StatusCode(500, "An error occurred while removing from cart");
            }
        }

        /// <summary>
        /// Clear all items from cart
        /// </summary>
        /// <returns>Success status</returns>
        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _cartService.ClearCartAsync(userId);
                if (!result)
                {
                    return BadRequest("Failed to clear cart");
                }

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing cart");
                return StatusCode(500, "An error occurred while clearing cart");
            }
        }

        /// <summary>
        /// Get cart item count
        /// </summary>
        /// <returns>Total number of items in cart</returns>
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var count = await _cartService.GetCartItemCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart item count");
                return StatusCode(500, "An error occurred while getting cart count");
            }
        }
    }
}
