using BookStore.API.DTOs;

namespace BookStore.API.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId);
        Task<bool> AddToFavoritesAsync(string userId, int bookId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int bookId);
        Task<bool> IsBookFavoriteAsync(string userId, int bookId);
    }
}
