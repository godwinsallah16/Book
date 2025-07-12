using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStore.API.Models;

namespace BookStore.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Book entity
            builder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
                entity.Property(b => b.PublicationYear).IsRequired();
                entity.Property(b => b.Publisher).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Category).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(b => b.StockQuantity).IsRequired();
                entity.Property(b => b.Description).HasMaxLength(1000);
                entity.Property(b => b.ImageUrl).HasMaxLength(500);
                entity.Property(b => b.CreatedAt).IsRequired();
                entity.Property(b => b.IsDeleted).HasDefaultValue(false);
                
                // Index for better performance
                entity.HasIndex(b => b.Category);
                entity.HasIndex(b => b.Title);
                entity.HasIndex(b => b.Author);
                entity.HasIndex(b => b.ISBN);
                entity.HasIndex(b => b.IsDeleted);
                entity.HasIndex(b => b.UserId);

                // Configure relationship with ApplicationUser
                entity.HasOne(b => b.User)
                    .WithMany(u => u.Books)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure UserFavorite entity
            builder.Entity<UserFavorite>(entity =>
            {
                entity.HasKey(uf => uf.Id);
                entity.Property(uf => uf.UserId).IsRequired();
                entity.Property(uf => uf.BookId).IsRequired();
                entity.Property(uf => uf.CreatedAt).IsRequired();

                // Configure relationships
                entity.HasOne(uf => uf.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(uf => uf.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uf => uf.Book)
                    .WithMany(b => b.Favorites)
                    .HasForeignKey(uf => uf.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ensure unique favorite per user per book
                entity.HasIndex(uf => new { uf.UserId, uf.BookId }).IsUnique();
            });

            // Configure CartItem entity
            builder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.Property(ci => ci.UserId).IsRequired();
                entity.Property(ci => ci.BookId).IsRequired();
                entity.Property(ci => ci.Quantity).IsRequired();
                entity.Property(ci => ci.CreatedAt).IsRequired();

                // Configure relationships
                entity.HasOne(ci => ci.User)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(ci => ci.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Book)
                    .WithMany(b => b.CartItems)
                    .HasForeignKey(ci => ci.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ensure unique cart item per user per book
                entity.HasIndex(ci => new { ci.UserId, ci.BookId }).IsUnique();
            });

            // Configure Order entity
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.UserId).IsRequired();
                entity.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(o => o.Status).IsRequired();
                entity.Property(o => o.PaymentMethod).IsRequired();
                entity.Property(o => o.CreatedAt).IsRequired();
                entity.Property(o => o.PaymentTransactionId).HasMaxLength(100);
                entity.Property(o => o.ShippingAddress).HasMaxLength(500);
                entity.Property(o => o.Notes).HasMaxLength(1000);

                // Configure relationships
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderItem entity
            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.OrderId).IsRequired();
                entity.Property(oi => oi.BookId).IsRequired();
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(oi => oi.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

                // Configure relationships
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Book)
                    .WithMany(b => b.OrderItems)
                    .HasForeignKey(oi => oi.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ApplicationUser entity
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(50);
                entity.Property(u => u.LastName).HasMaxLength(50);
                entity.Property(u => u.CreatedAt).IsRequired();
            });

            // Seed data
            SeedData(builder);
        }

        private static void SeedData(ModelBuilder builder)
        {
            // Use static datetime to avoid dynamic values warning
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            builder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "978-0743273565",
                    PublicationYear = 1925,
                    Publisher = "Scribner",
                    Category = "Classic Fiction",
                    Price = 12.99m,
                    StockQuantity = 50,
                    Description = "A classic American novel by F. Scott Fitzgerald",
                    ImageUrl = "",
                    CreatedAt = seedDate
                },
                new Book
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "978-0446310789",
                    PublicationYear = 1960,
                    Publisher = "J.B. Lippincott & Co.",
                    Category = "Classic Fiction",
                    Price = 14.99m,
                    StockQuantity = 30,
                    Description = "A novel by Harper Lee published in 1960",
                    ImageUrl = "",
                    CreatedAt = seedDate
                },
                new Book
                {
                    Id = 3,
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "978-0451524935",
                    PublicationYear = 1949,
                    Publisher = "Secker & Warburg",
                    Category = "Dystopian Fiction",
                    Price = 13.99m,
                    StockQuantity = 45,
                    Description = "A dystopian social science fiction novel by George Orwell",
                    ImageUrl = "",
                    CreatedAt = seedDate
                },
                new Book
                {
                    Id = 4,
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    ISBN = "978-0132350884",
                    PublicationYear = 2008,
                    Publisher = "Prentice Hall",
                    Category = "Technology",
                    Price = 39.99m,
                    StockQuantity = 25,
                    Description = "A handbook of agile software craftsmanship by Robert C. Martin",
                    ImageUrl = "",
                    CreatedAt = seedDate
                },
                new Book
                {
                    Id = 5,
                    Title = "Design Patterns",
                    Author = "Gang of Four",
                    ISBN = "978-0201633610",
                    PublicationYear = 1994,
                    Publisher = "Addison-Wesley",
                    Category = "Technology",
                    Price = 54.99m,
                    StockQuantity = 15,
                    Description = "Elements of Reusable Object-Oriented Software",
                    ImageUrl = "",
                    CreatedAt = seedDate
                }
            );
        }
    }
}
