using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UserId { get; set; }
        public string? UploaderName { get; set; }
        public bool IsFavorite { get; set; } = false;
    }

    public class CreateBookDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;
        
        [Required]
        [Range(1000, 9999, ErrorMessage = "Publication year must be between 1000 and current year")]
        public int PublicationYear { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Publisher { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, 99999.99, ErrorMessage = "Price must be between $0.01 and $99,999.99")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class UpdateBookDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;
        
        [Required]
        [Range(1000, 9999, ErrorMessage = "Publication year must be between 1000 and current year")]
        public int PublicationYear { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Publisher { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, 99999.99, ErrorMessage = "Price must be between $0.01 and $99,999.99")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
