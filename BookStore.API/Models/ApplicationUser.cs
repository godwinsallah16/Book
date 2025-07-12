using Microsoft.AspNetCore.Identity;

namespace BookStore.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
        public virtual ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
