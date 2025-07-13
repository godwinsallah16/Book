using BookStore.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.API.Repositories.Interfaces
{
    public interface IUserFavoriteRepository
    {
        Task<IEnumerable<UserFavorite>> GetFavoritesByUserAsync(string userId);
        Task<UserFavorite?> GetFavoriteAsync(string userId, int bookId);
        Task<UserFavorite> AddFavoriteAsync(string userId, int bookId);
        Task<bool> RemoveFavoriteAsync(string userId, int bookId);
    }
}
