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
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<FavoritesController> _logger;

        public FavoritesController(IFavoriteService favoriteService, ILogger<FavoritesController> logger)
        {
            _favoriteService = favoriteService;
            _logger = logger;
        }

        /// <summary>
        /// Get user's favorite books
        /// </summary>
        /// <returns>List of favorite books</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetUserFavorites()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user favorites");
                return StatusCode(500, "An error occurred while fetching favorites");
            }
        }

        /// <summary>
        /// Add book to favorites
        /// </summary>
        /// <param name="addToFavoriteDto">Book to add to favorites</param>
        /// <returns>Success status</returns>
        [HttpPost]
        public async Task<ActionResult> AddToFavorites([FromBody] AddToFavoriteDto addToFavoriteDto)
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

                var result = await _favoriteService.AddToFavoritesAsync(userId, addToFavoriteDto.BookId);
                if (!result)
                {
                    return NotFound("Book not found");
                }

                return Ok(new { message = "Book added to favorites" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book to favorites");
                return StatusCode(500, "An error occurred while adding to favorites");
            }
        }

        /// <summary>
        /// Remove book from favorites
        /// </summary>
        /// <param name="bookId">Book ID to remove from favorites</param>
        /// <returns>Success status</returns>
        [HttpDelete("{bookId}")]
        public async Task<ActionResult> RemoveFromFavorites(int bookId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _favoriteService.RemoveFromFavoritesAsync(userId, bookId);
                if (!result)
                {
                    return NotFound("Book not found in favorites");
                }

                return Ok(new { message = "Book removed from favorites" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing book from favorites");
                return StatusCode(500, "An error occurred while removing from favorites");
            }
        }

        /// <summary>
        /// Check if book is in user's favorites
        /// </summary>
        /// <param name="bookId">Book ID to check</param>
        /// <returns>True if book is in favorites</returns>
        [HttpGet("check/{bookId}")]
        public async Task<ActionResult<bool>> IsBookFavorite(int bookId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _favoriteService.IsBookFavoriteAsync(userId, bookId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if book is favorite");
                return StatusCode(500, "An error occurred while checking favorite status");
            }
        }
    }
}
