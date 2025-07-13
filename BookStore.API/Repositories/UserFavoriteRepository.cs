using BookStore.API.Data;
using BookStore.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.API.Repositories
{
    public class UserFavoriteRepository : Interfaces.IUserFavoriteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserFavoriteRepository> _logger;

        public UserFavoriteRepository(ApplicationDbContext context, ILogger<UserFavoriteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserFavorite>> GetFavoritesByUserAsync(string userId)
        {
            return await _context.UserFavorites
                .Include(f => f.Book)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserFavorite?> GetFavoriteAsync(string userId, int bookId)
        {
            return await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);
        }

        public async Task<UserFavorite> AddFavoriteAsync(string userId, int bookId)
        {
            var favorite = new UserFavorite { UserId = userId, BookId = bookId };
            _context.UserFavorites.Add(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, int bookId)
        {
            var favorite = await GetFavoriteAsync(userId, bookId);
            if (favorite == null) return false;
            _context.UserFavorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
