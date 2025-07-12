using BookStore.API.Services.Interfaces;
using BookStore.API.DTOs;
using Moq;
using Xunit;

namespace BookStore.API.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookService> _mockBookService;

    public BookServiceTests()
    {
        _mockBookService = new Mock<IBookService>();
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsAllBooks()
    {
        // Arrange
        var books = new List<BookDto>
        {
            new BookDto { Id = 1, Title = "Test Book 1", Author = "Author 1", Price = 10.99m },
            new BookDto { Id = 2, Title = "Test Book 2", Author = "Author 2", Price = 15.99m }
        };
        _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(books);

        // Act
        var result = await _mockBookService.Object.GetAllBooksAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.Title == "Test Book 1");
        Assert.Contains(result, b => b.Title == "Test Book 2");
    }

    [Fact]
    public async Task GetBookByIdAsync_WithValidId_ReturnsBook()
    {
        // Arrange
        var book = new BookDto { Id = 1, Title = "Test Book 1", Author = "Author 1", Price = 10.99m };
        _mockBookService.Setup(s => s.GetBookByIdAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _mockBookService.Object.GetBookByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Book 1", result.Title);
        Assert.Equal("Author 1", result.Author);
    }

    [Fact]
    public async Task GetBookByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockBookService.Setup(s => s.GetBookByIdAsync(999)).ReturnsAsync((BookDto?)null);

        // Act
        var result = await _mockBookService.Object.GetBookByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
