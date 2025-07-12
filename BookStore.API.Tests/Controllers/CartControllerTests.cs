using Microsoft.AspNetCore.Mvc;
using Moq;
using BookStore.API.Controllers;
using BookStore.API.Services.Interfaces;
using BookStore.API.DTOs;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookStore.API.Tests.Controllers;

public class CartControllerTests
{
    private readonly Mock<ICartService> _mockCartService;
    private readonly Mock<ILogger<CartController>> _mockLogger;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _mockCartService = new Mock<ICartService>();
        _mockLogger = new Mock<ILogger<CartController>>();
        _controller = new CartController(_mockCartService.Object, _mockLogger.Object);
        
        // Setup user context
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetUserCart_ReturnsOkResult_WithCartItems()
    {
        // Arrange
        var userId = "test-user-id";
        var cartItems = new List<CartItemDto>
        {
            new CartItemDto { Id = 1, BookId = 1, Quantity = 2 },
            new CartItemDto { Id = 2, BookId = 2, Quantity = 1 }
        };
        _mockCartService.Setup(s => s.GetUserCartAsync(userId)).ReturnsAsync(cartItems);

        // Act
        var result = await _controller.GetUserCart();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Assert.IsAssignableFrom<IEnumerable<CartItemDto>>(okResult.Value);
        Assert.Equal(2, returnedItems.Count());
    }

    [Fact]
    public async Task AddToCart_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var userId = "test-user-id";
        var request = new AddToCartDto { BookId = 1, Quantity = 2 };
        var cartItem = new CartItemDto { Id = 1, BookId = 1, Quantity = 2 };
        _mockCartService.Setup(s => s.AddToCartAsync(userId, request.BookId, request.Quantity))
                       .ReturnsAsync(cartItem);

        // Act
        var result = await _controller.AddToCart(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItem = Assert.IsType<CartItemDto>(okResult.Value);
        Assert.Equal(request.BookId, returnedItem.BookId);
        Assert.Equal(request.Quantity, returnedItem.Quantity);
    }
}
