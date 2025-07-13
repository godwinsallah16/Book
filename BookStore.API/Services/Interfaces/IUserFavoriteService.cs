using BookStore.API.DTOs;
using BookStore.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.API.Services.Interfaces
{
    public interface IUserFavoriteService
    {
        Task<IEnumerable<BookDto>> GetFavoriteBooksAsync(string userId);
        Task<bool> IsFavoriteAsync(string userId, int bookId);
        Task<bool> AddFavoriteAsync(string userId, int bookId);
        Task<bool> RemoveFavoriteAsync(string userId, int bookId);
    }
}
