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
                entity.Property(b => b.IsDeleted);

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
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.Entity<Book>().HasData(
                // Fiction - Romance
                new Book
                {
                    Id = 1,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ISBN = "978-0141439518",
                    PublicationYear = 1813,
                    Publisher = "Penguin Classics",
                    Category = "Fiction - Romance",
                    Price = 10.99m,
                    StockQuantity = 40,
                    Description = "A classic romance novel exploring societal expectations and true love in 19th century England.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231852-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 2,
                    Title = "The Fault in Our Stars",
                    Author = "John Green",
                    ISBN = "978-0525478812",
                    PublicationYear = 2012,
                    Publisher = "Dutton Books",
                    Category = "Fiction - Romance",
                    Price = 12.99m,
                    StockQuantity = 35,
                    Description = "Two teenagers with cancer fall in love while navigating life's harsh realities.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231863-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                // Fiction - Thriller
                new Book
                {
                    Id = 3,
                    Title = "The Girl with the Dragon Tattoo",
                    Author = "Stieg Larsson",
                    ISBN = "978-0307454546",
                    PublicationYear = 2005,
                    Publisher = "Norstedts Förlag",
                    Category = "Fiction - Thriller",
                    Price = 13.99m,
                    StockQuantity = 30,
                    Description = "A journalist and hacker investigate a decades-old disappearance in Sweden.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/240726-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 4,
                    Title = "Gone Girl",
                    Author = "Gillian Flynn",
                    ISBN = "978-0307588371",
                    PublicationYear = 2012,
                    Publisher = "Crown Publishing",
                    Category = "Fiction - Thriller",
                    Price = 14.99m,
                    StockQuantity = 25,
                    Description = "A psychological thriller about a marriage gone terribly wrong.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235119-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                // Fiction - Fantasy
                new Book
                {
                    Id = 5,
                    Title = "Harry Potter and the Sorcerer's Stone",
                    Author = "J.K. Rowling",
                    ISBN = "978-0590353427",
                    PublicationYear = 1997,
                    Publisher = "Scholastic",
                    Category = "Fiction - Fantasy",
                    Price = 9.99m,
                    StockQuantity = 50,
                    Description = "The first book in the magical series about a young wizard.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/7984916-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 6,
                    Title = "The Night Circus",
                    Author = "Erin Morgenstern",
                    ISBN = "978-0307744432",
                    PublicationYear = 2011,
                    Publisher = "Doubleday",
                    Category = "Fiction - Fantasy",
                    Price = 15.99m,
                    StockQuantity = 20,
                    Description = "A magical competition between two illusionists in a mysterious circus.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235120-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 13,
                    Title = "Steve Jobs",
                    Author = "Walter Isaacson",
                    ISBN = "978-1451648539",
                    PublicationYear = 2011,
                    Publisher = "Simon & Schuster",
                    Category = "Non-Fiction - Biography",
                    Price = 18.99m,
                    StockQuantity = 15,
                    Description = "The authorized biography of Apple's co-founder.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/7265441-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 14,
                    Title = "Becoming",
                    Author = "Michelle Obama",
                    ISBN = "978-1524763138",
                    PublicationYear = 2018,
                    Publisher = "Crown",
                    Category = "Non-Fiction - Biography",
                    Price = 19.99m,
                    StockQuantity = 20,
                    Description = "Memoir of the former First Lady of the United States.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235124-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 15,
                    Title = "Atomic Habits",
                    Author = "James Clear",
                    ISBN = "978-0735211292",
                    PublicationYear = 2018,
                    Publisher = "Avery",
                    Category = "Non-Fiction - Self-Help",
                    Price = 16.99m,
                    StockQuantity = 25,
                    Description = "A guide to building good habits and breaking bad ones.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/9259256-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 16,
                    Title = "Sapiens: A Brief History of Humankind",
                    Author = "Yuval Noah Harari",
                    ISBN = "978-0062316097",
                    PublicationYear = 2011,
                    Publisher = "Harper",
                    Category = "Non-Fiction - History",
                    Price = 19.99m,
                    StockQuantity = 20,
                    Description = "Exploration of human history from evolution to modern society.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231857-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 17,
                    Title = "Killers of the Flower Moon",
                    Author = "David Grann",
                    ISBN = "978-0385534246",
                    PublicationYear = 2017,
                    Publisher = "Doubleday",
                    Category = "Non-Fiction - History",
                    Price = 17.99m,
                    StockQuantity = 15,
                    Description = "The Osage murders and birth of the FBI.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235125-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 18,
                    Title = "The Audacity of Hope",
                    Author = "Barack Obama",
                    ISBN = "978-0307237705",
                    PublicationYear = 2006,
                    Publisher = "Crown",
                    Category = "Non-Fiction - Politics",
                    Price = 14.99m,
                    StockQuantity = 18,
                    Description = "Thoughts on reclaiming the American dream.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231858-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 19,
                    Title = "In Cold Blood",
                    Author = "Truman Capote",
                    ISBN = "978-0679745587",
                    PublicationYear = 1966,
                    Publisher = "Vintage",
                    Category = "Non-Fiction - True Crime",
                    Price = 13.99m,
                    StockQuantity = 12,
                    Description = "True crime classic about the Clutter family murders.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231859-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 20,
                    Title = "The Purpose Driven Life",
                    Author = "Rick Warren",
                    ISBN = "978-0310337508",
                    PublicationYear = 2002,
                    Publisher = "Zondervan",
                    Category = "Non-Fiction - Religion",
                    Price = 12.99m,
                    StockQuantity = 14,
                    Description = "Spiritual guide to finding purpose through faith.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231860-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Children & Teens - Picture Books
                new Book
                {
                    Id = 21,
                    Title = "The Very Hungry Caterpillar",
                    Author = "Eric Carle",
                    ISBN = "978-0399226908",
                    PublicationYear = 1969,
                    Publisher = "Philomel Books",
                    Category = "Children & Teens - Picture Books",
                    Price = 8.99m,
                    StockQuantity = 30,
                    Description = "A caterpillar eats his way through various foods.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231861-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Children & Teens - Middle Grade
                new Book
                {
                    Id = 22,
                    Title = "Percy Jackson & the Olympians: The Lightning Thief",
                    Author = "Rick Riordan",
                    ISBN = "978-0786838653",
                    PublicationYear = 2005,
                    Publisher = "Disney-Hyperion",
                    Category = "Children & Teens - Middle Grade",
                    Price = 9.99m,
                    StockQuantity = 22,
                    Description = "A modern demigod discovers his divine heritage.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231862-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 23,
                    Title = "Wonder",
                    Author = "R.J. Palacio",
                    ISBN = "978-0375869020",
                    PublicationYear = 2012,
                    Publisher = "Knopf",
                    Category = "Children & Teens - Middle Grade",
                    Price = 10.99m,
                    StockQuantity = 18,
                    Description = "A boy with facial differences attends mainstream school.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235126-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Children & Teens - Young Adult
                new Book
                {
                    Id = 24,
                    Title = "The Hate U Give",
                    Author = "Angie Thomas",
                    ISBN = "978-0062498533",
                    PublicationYear = 2017,
                    Publisher = "Balzer + Bray",
                    Category = "Children & Teens - Young Adult",
                    Price = 12.99m,
                    StockQuantity = 20,
                    Description = "A teen witnesses police shooting her childhood friend.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235127-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Book
                {
                    Id = 25,
                    Title = "Divergent",
                    Author = "Veronica Roth",
                    ISBN = "978-0062024039",
                    PublicationYear = 2011,
                    Publisher = "Katherine Tegen Books",
                    Category = "Children & Teens - Young Adult",
                    Price = 11.99m,
                    StockQuantity = 16,
                    Description = "Dystopian Chicago where society is divided into factions.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8235128-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Academic & Textbooks - Science
                new Book
                {
                    Id = 26,
                    Title = "A Brief History of Time",
                    Author = "Stephen Hawking",
                    ISBN = "978-0553380163",
                    PublicationYear = 1988,
                    Publisher = "Bantam",
                    Category = "Academic & Textbooks - Science",
                    Price = 17.99m,
                    StockQuantity = 10,
                    Description = "Introduction to cosmology for non-specialists.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231864-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Academic & Textbooks - Engineering
                new Book
                {
                    Id = 27,
                    Title = "Structures: Or Why Things Don't Fall Down",
                    Author = "J.E. Gordon",
                    ISBN = "978-0306812837",
                    PublicationYear = 1978,
                    Publisher = "Da Capo Press",
                    Category = "Academic & Textbooks - Engineering",
                    Price = 15.99m,
                    StockQuantity = 8,
                    Description = "Fundamentals of structural engineering made accessible.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231865-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Academic & Textbooks - Medicine
                new Book
                {
                    Id = 28,
                    Title = "The Emperor of All Maladies",
                    Author = "Siddhartha Mukherjee",
                    ISBN = "978-1439170915",
                    PublicationYear = 2010,
                    Publisher = "Scribner",
                    Category = "Academic & Textbooks - Medicine",
                    Price = 18.99m,
                    StockQuantity = 7,
                    Description = "Biography of cancer spanning centuries of research.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231866-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Academic & Textbooks - Law
                new Book
                {
                    Id = 29,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "978-0060935467",
                    PublicationYear = 1960,
                    Publisher = "J.B. Lippincott & Co.",
                    Category = "Academic & Textbooks - Law",
                    Price = 14.99m,
                    StockQuantity = 9,
                    Description = "Exploration of racial injustice and moral growth.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8228692-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Academic & Textbooks - Test Prep
                new Book
                {
                    Id = 30,
                    Title = "Cracking the SAT",
                    Author = "The Princeton Review",
                    ISBN = "978-0525568063",
                    PublicationYear = 2020,
                    Publisher = "Princeton Review",
                    Category = "Academic & Textbooks - Test Prep",
                    Price = 21.99m,
                    StockQuantity = 12,
                    Description = "Comprehensive SAT preparation guide with practice tests.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231867-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Comics & Graphic Novels - Manga
                new Book
                {
                    Id = 31,
                    Title = "Naruto, Vol. 1",
                    Author = "Masashi Kishimoto",
                    ISBN = "978-1569319000",
                    PublicationYear = 2003,
                    Publisher = "VIZ Media",
                    Category = "Comics & Graphic Novels - Manga",
                    Price = 9.99m,
                    StockQuantity = 20,
                    Description = "A young ninja dreams of becoming his village's leader.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231868-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Comics & Graphic Novels - Superhero
                new Book
                {
                    Id = 32,
                    Title = "Batman: Year One",
                    Author = "Frank Miller",
                    ISBN = "978-1401207526",
                    PublicationYear = 1987,
                    Publisher = "DC Comics",
                    Category = "Comics & Graphic Novels - Superhero",
                    Price = 12.99m,
                    StockQuantity = 15,
                    Description = "Batman's early crime-fighting days in Gotham City.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231869-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Comics & Graphic Novels - Indie
                new Book
                {
                    Id = 33,
                    Title = "Persepolis",
                    Author = "Marjane Satrapi",
                    ISBN = "978-0375714573",
                    PublicationYear = 2000,
                    Publisher = "Pantheon",
                    Category = "Comics & Graphic Novels - Indie",
                    Price = 13.99m,
                    StockQuantity = 10,
                    Description = "Graphic memoir of growing up during Iranian Revolution.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231870-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Comics & Graphic Novels - Webtoons
                new Book
                {
                    Id = 34,
                    Title = "Lore Olympus: Volume One",
                    Author = "Rachel Smythe",
                    ISBN = "978-0593160290",
                    PublicationYear = 2021,
                    Publisher = "Del Rey",
                    Category = "Comics & Graphic Novels - Webtoons",
                    Price = 16.99m,
                    StockQuantity = 8,
                    Description = "Modern retelling of Hades and Persephone myth.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231871-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Lifestyle & Hobbies - Cooking
                new Book
                {
                    Id = 35,
                    Title = "Salt, Fat, Acid, Heat",
                    Author = "Samin Nosrat",
                    ISBN = "978-1476753836",
                    PublicationYear = 2017,
                    Publisher = "Simon & Schuster",
                    Category = "Lifestyle & Hobbies - Cooking",
                    Price = 24.99m,
                    StockQuantity = 10,
                    Description = "Mastering the elements of good cooking.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231872-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },

                // Lifestyle & Hobbies - Travel
                new Book
                {
                    Id = 36,
                    Title = "Lonely Planet's Ultimate Travel List",
                    Author = "Lonely Planet",
                    ISBN = "978-1788689199",
                    PublicationYear = 2020,
                    Publisher = "Lonely Planet",
                    Category = "Lifestyle & Hobbies - Travel",
                    Price = 29.99m,
                    StockQuantity = 7,
                    Description = "Ranking of the world's top travel experiences.",
                    ImageUrl = "https://covers.openlibrary.org/b/id/8231873-L.jpg",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }
    } 
}
