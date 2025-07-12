using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public BookDto Book { get; set; } = null!;
    }

    public class AddToCartDto
    {
        [Required]
        public int BookId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
