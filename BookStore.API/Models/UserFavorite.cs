using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.API.Models
{
    public class UserFavorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BookId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; } = null!;
    }
}
