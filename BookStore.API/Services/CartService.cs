using AutoMapper;
using BookStore.API.DTOs;
using BookStore.API.Models;
using BookStore.API.Services.Interfaces;
using BookStore.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext context, IMapper mapper, ILogger<CartService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CartItemDto>> GetUserCartAsync(string userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Include(c => c.Book)
                    .ThenInclude(b => b.User)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var cartItemDtos = cartItems.Select(c => new CartItemDto
                {
                    Id = c.Id,
                    BookId = c.BookId,
                    Quantity = c.Quantity,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Book = _mapper.Map<BookDto>(c.Book)
                });

                return cartItemDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<CartItemDto?> AddToCartAsync(string userId, int bookId, int quantity)
        {
            try
            {
                // Check if user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    _logger.LogWarning("Cannot add to cart: user {UserId} does not exist", userId);
                    return null;
                }

                // Check if book exists and is available
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.IsDeleted || book.StockQuantity < quantity)
                {
                    return null;
                }

                // Check if item already exists in cart
                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

                if (existingCartItem != null)
                {
                    // Update quantity
                    existingCartItem.Quantity += quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new cart item
                    existingCartItem = new CartItem
                    {
                        UserId = userId,
                        BookId = bookId,
                        Quantity = quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.CartItems.Add(existingCartItem);
                }

                await _context.SaveChangesAsync();

                // Return the cart item with book details
                var cartItemWithBook = await _context.CartItems
                    .Include(c => c.Book)
                    .ThenInclude(b => b.User)
                    .FirstOrDefaultAsync(c => c.Id == existingCartItem.Id);

                var cartItemDto = new CartItemDto
                {
                    Id = cartItemWithBook!.Id,
                    BookId = cartItemWithBook.BookId,
                    Quantity = cartItemWithBook.Quantity,
                    CreatedAt = cartItemWithBook.CreatedAt,
                    UpdatedAt = cartItemWithBook.UpdatedAt,
                    Book = _mapper.Map<BookDto>(cartItemWithBook.Book)
                };

                _logger.LogInformation("Book {BookId} added to cart for user {UserId}", bookId, userId);
                return cartItemDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book {BookId} to cart for user {UserId}", bookId, userId);
                return null;
            }
        }

        public async Task<CartItemDto?> UpdateCartItemAsync(string userId, int bookId, int quantity)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(c => c.Book)
                    .ThenInclude(b => b.User)
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

                if (cartItem == null)
                {
                    return null;
                }

                // Check if book has enough stock
                if (cartItem.Book.StockQuantity < quantity)
                {
                    return null;
                }

                cartItem.Quantity = quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var cartItemDto = new CartItemDto
                {
                    Id = cartItem.Id,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    CreatedAt = cartItem.CreatedAt,
                    UpdatedAt = cartItem.UpdatedAt,
                    Book = _mapper.Map<BookDto>(cartItem.Book)
                };

                _logger.LogInformation("Cart item {CartItemId} updated for user {UserId}", cartItem.Id, userId);
                return cartItemDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart item for book {BookId} for user {UserId}", bookId, userId);
                return null;
            }
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int bookId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

                if (cartItem == null)
                {
                    return false;
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book {BookId} removed from cart for user {UserId}", bookId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing book {BookId} from cart for user {UserId}", bookId, userId);
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cart cleared for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing cart for user {UserId}", userId);
                return false;
            }
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            try
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart item count for user {UserId}", userId);
                return 0;
            }
        }
    }
}
