using BookStore.API.DTOs;

namespace BookStore.API.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetUserCartAsync(string userId);
        Task<CartItemDto?> AddToCartAsync(string userId, int bookId, int quantity);
        Task<CartItemDto?> UpdateCartItemAsync(string userId, int bookId, int quantity);
        Task<bool> RemoveFromCartAsync(string userId, int bookId);
        Task<bool> ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
    }
}
