using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookDto Book { get; set; } = null!;
    }

    public class AddToFavoriteDto
    {
        [Required]
        public int BookId { get; set; }
    }

    public class RemoveFromFavoriteDto
    {
        [Required]
        public int BookId { get; set; }
    }
}
