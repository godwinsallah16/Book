using BookStore.API.Models;

namespace BookStore.API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> GetBooksPaginatedWithCountAsync(int page, int pageSize, BookStore.API.DTOs.BookFiltersDto? filters = null);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book?> GetBookByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
        Task<Book> CreateBookAsync(Book book);
        Task<Book?> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<string>> GetAuthorsAsync();
        Task<IEnumerable<Book>> GetUserBooksAsync(string userId);
    }
}
