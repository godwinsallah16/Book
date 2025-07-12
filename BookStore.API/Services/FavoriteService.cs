using AutoMapper;
using BookStore.API.DTOs;
using BookStore.API.Models;
using BookStore.API.Services.Interfaces;
using BookStore.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(ApplicationDbContext context, IMapper mapper, ILogger<FavoriteService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId)
        {
            try
            {
                var favorites = await _context.UserFavorites
                    .Include(f => f.Book)
                    .ThenInclude(b => b.User)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                var favoriteDtos = favorites.Select(f => new FavoriteDto
                {
                    Id = f.Id,
                    BookId = f.BookId,
                    CreatedAt = f.CreatedAt,
                    Book = _mapper.Map<BookDto>(f.Book)
                });

                return favoriteDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching favorites for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int bookId)
        {
            try
            {
                // Check if book exists
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.IsDeleted)
                {
                    return false;
                }

                // Check if already in favorites
                var existingFavorite = await _context.UserFavorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

                if (existingFavorite != null)
                {
                    return true; // Already in favorites
                }

                var favorite = new UserFavorite
                {
                    UserId = userId,
                    BookId = bookId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserFavorites.Add(favorite);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book {BookId} added to favorites for user {UserId}", bookId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book {BookId} to favorites for user {UserId}", bookId, userId);
                return false;
            }
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int bookId)
        {
            try
            {
                var favorite = await _context.UserFavorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

                if (favorite == null)
                {
                    return false;
                }

                _context.UserFavorites.Remove(favorite);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book {BookId} removed from favorites for user {UserId}", bookId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing book {BookId} from favorites for user {UserId}", bookId, userId);
                return false;
            }
        }

        public async Task<bool> IsBookFavoriteAsync(string userId, int bookId)
        {
            try
            {
                return await _context.UserFavorites
                    .AnyAsync(f => f.UserId == userId && f.BookId == bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if book {BookId} is favorite for user {UserId}", bookId, userId);
                return false;
            }
        }
    }
}
