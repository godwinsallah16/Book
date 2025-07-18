using BookStore.API.DTOs;
using BookStore.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.API.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUserFavoriteService _userFavoriteService;

        public FavoriteService(IUserFavoriteService userFavoriteService)
        {
            _userFavoriteService = userFavoriteService;
        }

        public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId)
        {
            var bookDtos = await _userFavoriteService.GetFavoriteBooksAsync(userId);
            var favoriteDtos = new List<FavoriteDto>();
            foreach (var bookDto in bookDtos)
            {
                favoriteDtos.Add(new FavoriteDto
                {
                    Id = 0, // Id unknown here
                    BookId = bookDto.Id,
                    CreatedAt = System.DateTime.MinValue, // Unknown created date
                    Book = bookDto
                });
            }
            return favoriteDtos;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int bookId)
        {
            return await _userFavoriteService.AddFavoriteAsync(userId, bookId);
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int bookId)
        {
            return await _userFavoriteService.RemoveFavoriteAsync(userId, bookId);
        }

        public async Task<bool> IsBookFavoriteAsync(string userId, int bookId)
        {
            return await _userFavoriteService.IsFavoriteAsync(userId, bookId);
        }
    }
}
