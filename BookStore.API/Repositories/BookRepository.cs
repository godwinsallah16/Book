using Microsoft.EntityFrameworkCore;
using BookStore.API.Data;
using BookStore.API.Models;
using BookStore.API.Repositories.Interfaces;

namespace BookStore.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(ApplicationDbContext context, ILogger<BookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetBooksPaginatedWithCountAsync(int page, int pageSize)
        {
            try
            {
                var query = _context.Books.Where(b => !b.IsDeleted);
                var totalCount = await query.CountAsync();
                var books = await query
                    .OrderBy(b => b.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return (books, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching paginated books");
                throw;
            }
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            try
            {
                return await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<Book?> GetBookByIdIncludingDeletedAsync(int id)
        {
            try
            {
                return await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id); // Include soft-deleted books
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book with ID {BookId} (including deleted)", id);
                throw;
            }
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    var (books, _) = await GetBooksPaginatedWithCountAsync(1, 20); // Default page and size
                    return books;
                }

                var lowerSearchTerm = searchTerm.ToLower();
                return await _context.Books
                    .Where(b => !b.IsDeleted &&
                        (b.Title.ToLower().Contains(lowerSearchTerm) ||
                         b.Author.ToLower().Contains(lowerSearchTerm) ||
                         b.ISBN.ToLower().Contains(lowerSearchTerm) ||
                         b.Description.ToLower().Contains(lowerSearchTerm) ||
                         b.Category.ToLower().Contains(lowerSearchTerm)))
                    .OrderBy(b => b.Title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching books with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category)
        {
            try
            {
                return await _context.Books
                    .Where(b => !b.IsDeleted && b.Category.ToLower() == category.ToLower())
                    .OrderBy(b => b.Title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books by category '{Category}'", category);
                throw;
            }
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            try
            {
                book.CreatedAt = DateTime.UtcNow;
                book.IsDeleted = false;
                
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Book created successfully with ID {BookId}", book.Id);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating book '{BookTitle}'", book.Title);
                throw;
            }
        }

        public async Task<Book?> UpdateBookAsync(int id, Book book)
        {
            try
            {
                var existingBook = await GetBookByIdAsync(id);
                if (existingBook == null)
                    return null;

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.PublicationYear = book.PublicationYear;
                existingBook.Publisher = book.Publisher;
                existingBook.Category = book.Category;
                existingBook.Price = book.Price;
                existingBook.StockQuantity = book.StockQuantity;
                existingBook.Description = book.Description;
                existingBook.ImageUrl = book.ImageUrl;
                existingBook.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Book updated successfully with ID {BookId}", id);
                return existingBook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await GetBookByIdAsync(id);
                if (book == null)
                    return false;

                book.IsDeleted = true;
                book.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Book soft deleted successfully with ID {BookId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            try
            {
                return await _context.Books
                    .AnyAsync(b => b.Id == id && !b.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if book exists with ID {BookId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Books
                    .Where(b => !b.IsDeleted)
                    .Select(b => b.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book categories");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAuthorsAsync()
        {
            try
            {
                return await _context.Books
                    .Where(b => !b.IsDeleted)
                    .Select(b => b.Author)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book authors");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetUserBooksAsync(string userId)
        {
            try
            {
                return await _context.Books
                    .Include(b => b.User)
                    .Where(b => !b.IsDeleted && b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books for user {UserId}", userId);
                throw;
            }
        }
    }
}
