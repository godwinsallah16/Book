using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStore.API.DTOs;
using BookStore.API.Services.Interfaces;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <returns>List of books</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books");
                return StatusCode(500, "An error occurred while fetching books");
            }
        }

        /// <summary>
        /// Get book by ID
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Book details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book with ID {BookId}", id);
                return StatusCode(500, "An error occurred while fetching the book");
            }
        }

        /// <summary>
        /// Search books by name, description, or category
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching books</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string searchTerm)
        {
            try
            {
                var books = await _bookService.SearchBooksAsync(searchTerm);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching books with term '{SearchTerm}'", searchTerm);
                return StatusCode(500, "An error occurred while searching books");
            }
        }

        /// <summary>
        /// Get books by category
        /// </summary>
        /// <param name="category">Book category</param>
        /// <returns>List of books in the category</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(string category)
        {
            try
            {
                var books = await _bookService.GetBooksByCategoryAsync(category);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books by category '{Category}'", category);
                return StatusCode(500, "An error occurred while fetching books by category");
            }
        }

        /// <summary>
        /// Get all book categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _bookService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book categories");
                return StatusCode(500, "An error occurred while fetching categories");
            }
        }

        /// <summary>
        /// Get all book authors
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet("authors")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetAuthors()
        {
            try
            {
                var authors = await _bookService.GetAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book authors");
                return StatusCode(500, "An error occurred while fetching authors");
            }
        }

        /// <summary>
        /// Create a new book
        /// </summary>
        /// <param name="createBookDto">Book creation details</param>
        /// <returns>Created book</returns>
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var book = await _bookService.CreateBookAsync(createBookDto, userId);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating book '{BookTitle}'", createBookDto.Title);
                return StatusCode(500, "An error occurred while creating the book");
            }
        }

        /// <summary>
        /// Update an existing book
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <param name="updateBookDto">Book update details</param>
        /// <returns>Updated book</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var isAdmin = User.IsInRole("Admin");
                var book = await _bookService.UpdateBookAsync(id, updateBookDto, userId, isAdmin);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found or you don't have permission to update it");
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID {BookId}", id);
                return StatusCode(500, "An error occurred while updating the book");
            }
        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var isAdmin = User.IsInRole("Admin");
                var result = await _bookService.DeleteBookAsync(id, userId, isAdmin);
                if (!result)
                {
                    return NotFound($"Book with ID {id} not found or you don't have permission to delete it");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with ID {BookId}", id);
                return StatusCode(500, "An error occurred while deleting the book");
            }
        }

        /// <summary>
        /// Get books uploaded by the current user
        /// </summary>
        /// <returns>List of user's books</returns>
        [HttpGet("my-books")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetUserBooks()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var books = await _bookService.GetUserBooksAsync(userId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user books");
                return StatusCode(500, "An error occurred while fetching your books");
            }
        }
    }
}
