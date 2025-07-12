using AutoMapper;
using BookStore.API.DTOs;
using BookStore.API.Models;
using BookStore.API.Repositories.Interfaces;
using BookStore.API.Services.Interfaces;

namespace BookStore.API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            try
            {
                var books = await _bookRepository.GetAllBooksAsync();
                return _mapper.Map<IEnumerable<BookDto>>(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all books");
                throw;
            }
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);
                return book == null ? null : _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            try
            {
                var books = await _bookRepository.SearchBooksAsync(searchTerm);
                return _mapper.Map<IEnumerable<BookDto>>(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching books with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category)
        {
            try
            {
                var books = await _bookRepository.GetBooksByCategoryAsync(category);
                return _mapper.Map<IEnumerable<BookDto>>(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting books by category '{Category}'", category);
                throw;
            }
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, string userId)
        {
            try
            {
                var book = _mapper.Map<Book>(createBookDto);
                book.UserId = userId; // Set the user who uploaded the book
                var createdBook = await _bookRepository.CreateBookAsync(book);
                return _mapper.Map<BookDto>(createdBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating book '{BookTitle}'", createBookDto.Title);
                throw;
            }
        }

        public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto updateBookDto, string userId, bool isAdmin = false)
        {
            try
            {
                // Check if user can edit this book
                if (!await CanUserEditBookAsync(id, userId, isAdmin))
                {
                    return null;
                }

                var book = _mapper.Map<Book>(updateBookDto);
                var updatedBook = await _bookRepository.UpdateBookAsync(id, book);
                return updatedBook == null ? null : _mapper.Map<BookDto>(updatedBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id, string userId, bool isAdmin = false)
        {
            try
            {
                // First, check if the book exists (including soft-deleted books)
                var book = await _bookRepository.GetBookByIdIncludingDeletedAsync(id);
                if (book == null)
                {
                    return false; // Book doesn't exist at all
                }

                // If book is already soft-deleted, consider it already deleted
                if (book.IsDeleted)
                {
                    return true; // Already deleted, return success
                }

                // Check if user can delete this book
                if (!await CanUserEditBookAsync(id, userId, isAdmin))
                {
                    return false;
                }

                return await _bookRepository.DeleteBookAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                return await _bookRepository.GetCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting book categories");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAuthorsAsync()
        {
            try
            {
                return await _bookRepository.GetAuthorsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting book authors");
                throw;
            }
        }

        public async Task<IEnumerable<BookDto>> GetUserBooksAsync(string userId)
        {
            try
            {
                var books = await _bookRepository.GetUserBooksAsync(userId);
                return _mapper.Map<IEnumerable<BookDto>>(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting books for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> CanUserEditBookAsync(int bookId, string userId, bool isAdmin = false)
        {
            try
            {
                // Admins can edit any book
                if (isAdmin)
                {
                    return true;
                }

                // Check if user owns the book
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                return book != null && book.UserId == userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking edit permissions for book {BookId} and user {UserId}", bookId, userId);
                return false;
            }
        }
    }
}
