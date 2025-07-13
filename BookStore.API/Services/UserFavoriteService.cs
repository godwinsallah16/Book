using BookStore.API.DTOs;
using BookStore.API.Models;
using BookStore.API.Repositories.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.API.Services
{
    public class UserFavoriteService : Interfaces.IUserFavoriteService
    {
        private readonly IUserFavoriteRepository _favoriteRepository;
        private readonly IMapper _mapper;

        public UserFavoriteService(IUserFavoriteRepository favoriteRepository, IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetFavoriteBooksAsync(string userId)
        {
            var favorites = await _favoriteRepository.GetFavoritesByUserAsync(userId);
            return favorites.Select(f => _mapper.Map<BookDto>(f.Book));
        }

        public async Task<bool> IsFavoriteAsync(string userId, int bookId)
        {
            var fav = await _favoriteRepository.GetFavoriteAsync(userId, bookId);
            return fav != null;
        }

        public async Task<bool> AddFavoriteAsync(string userId, int bookId)
        {
            var exists = await _favoriteRepository.GetFavoriteAsync(userId, bookId);
            if (exists != null) return false;
            await _favoriteRepository.AddFavoriteAsync(userId, bookId);
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, int bookId)
        {
            return await _favoriteRepository.RemoveFavoriteAsync(userId, bookId);
        }
    }
}
