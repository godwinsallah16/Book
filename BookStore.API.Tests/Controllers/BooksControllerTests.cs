using Microsoft.AspNetCore.Mvc;
using Moq;
using BookStore.API.Controllers;
using BookStore.API.Services.Interfaces;
using BookStore.API.DTOs;
using Xunit;
using Microsoft.Extensions.Logging;

namespace BookStore.API.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly Mock<ILogger<BooksController>> _mockLogger;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _mockLogger = new Mock<ILogger<BooksController>>();
        _controller = new BooksController(_mockBookService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetBooks_ReturnsOkResult_WithBooks()
    {
        // Arrange
        var books = new List<BookDto>
        {
            new BookDto { Id = 1, Title = "Test Book 1", Author = "Author 1", Price = 10.99m },
            new BookDto { Id = 2, Title = "Test Book 2", Author = "Author 2", Price = 15.99m }
        };
        _mockBookService.Setup(s => s.GetBooksPaginatedWithCountAsync(1, 20, It.IsAny<BookFiltersDto>()))
            .ReturnsAsync((books, books.Count));

        // Act
        var result = await _controller.GetBooks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        // The controller returns an anonymous type with data property containing books
        var anonymousResult = okResult.Value;
        var dataProperty = anonymousResult.GetType().GetProperty("data");
        Assert.NotNull(dataProperty);
        var returnedBooks = dataProperty.GetValue(anonymousResult) as IEnumerable<BookDto>;
        Assert.NotNull(returnedBooks);
        Assert.Equal(2, returnedBooks.Count());
    }

    [Fact]
    public async Task GetBook_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var bookId = 1;
        var book = new BookDto { Id = bookId, Title = "Test Book", Author = "Test Author", Price = 10.99m };
        _mockBookService.Setup(s => s.GetBookByIdAsync(bookId)).ReturnsAsync(book);

        // Act
        var result = await _controller.GetBook(bookId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBook = Assert.IsType<BookDto>(okResult.Value);
        Assert.Equal(bookId, returnedBook.Id);
    }

    [Fact]
    public async Task GetBook_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var bookId = 999;
        _mockBookService.Setup(s => s.GetBookByIdAsync(bookId)).ReturnsAsync((BookDto?)null);

        // Act
        var result = await _controller.GetBook(bookId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}
