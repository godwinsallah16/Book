using BookStore.API.DTOs;
using BookStore.API.Models;

namespace BookStore.API.Services.Interfaces
{
    public interface IBookService
    {
        Task<(IEnumerable<BookDto> Books, int TotalCount)> GetBooksPaginatedWithCountAsync(int page, int pageSize);
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, string userId);
        Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto updateBookDto, string userId, bool isAdmin = false);
        Task<bool> DeleteBookAsync(int id, string userId, bool isAdmin = false);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<string>> GetAuthorsAsync();
        Task<IEnumerable<BookDto>> GetUserBooksAsync(string userId);
        Task<bool> CanUserEditBookAsync(int bookId, string userId, bool isAdmin = false);
    }
}
