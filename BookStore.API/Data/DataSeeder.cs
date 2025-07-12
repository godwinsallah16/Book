using Microsoft.AspNetCore.Identity;
using BookStore.API.Models;

namespace BookStore.API.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Create roles
            await CreateRolesAsync();

            // Create admin user
            await CreateAdminUserAsync();

            // Seed sample books
            await SeedBooksAsync();
        }

        private async Task CreateRolesAsync()
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task CreateAdminUserAsync()
        {
            var adminEmail = "godwinsallah16@gmail.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Godwin",
                    LastName = "Sallah",
                    EmailConfirmed = true, // Admin user is pre-verified
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@bookstore");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                // Update existing admin user to ensure they have the correct role
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private async Task SeedBooksAsync()
        {
            if (!_context.Books.Any())
            {
                var books = new[]
                {
                    new Book
                    {
                        Title = "Red Pyramid",
                        Author = "Rick Riordan",
                        ISBN = "978-1423113386",
                        PublicationYear = 2010,
                        Publisher = "Disney Hyperion",
                        Category = "Fantasy",
                        Price = 12.99m,
                        StockQuantity = 50,
                        Description = "The first book in the Kane Chronicles series by Rick Riordan. Follow Carter and Sadie Kane as they discover their family's connection to the House of Life and Egyptian magic.",
                        ImageUrl = "https://example.com/red-pyramid.jpg"
                    },
                    new Book
                    {
                        Title = "The Throne of Fire",
                        Author = "Rick Riordan",
                        ISBN = "978-1423140566",
                        PublicationYear = 2011,
                        Publisher = "Disney Hyperion",
                        Category = "Fantasy",
                        Price = 13.99m,
                        StockQuantity = 35,
                        Description = "The second book in the Kane Chronicles series. The Kane siblings continue their battle against the forces of chaos.",
                        ImageUrl = "https://example.com/throne-of-fire.jpg"
                    },
                    new Book
                    {
                        Title = "The Serpent's Shadow",
                        Author = "Rick Riordan",
                        ISBN = "978-1423140573",
                        PublicationYear = 2012,
                        Publisher = "Disney Hyperion",
                        Category = "Fantasy",
                        Price = 14.99m,
                        StockQuantity = 30,
                        Description = "The final book in the Kane Chronicles series. Carter and Sadie Kane face their greatest challenge yet.",
                        ImageUrl = "https://example.com/serpents-shadow.jpg"
                    },
                    new Book
                    {
                        Title = "Percy Jackson: The Lightning Thief",
                        Author = "Rick Riordan",
                        ISBN = "978-0786838653",
                        PublicationYear = 2005,
                        Publisher = "Disney Hyperion",
                        Category = "Fantasy",
                        Price = 11.99m,
                        StockQuantity = 45,
                        Description = "The first book in the Percy Jackson series. Percy discovers he's a demigod and embarks on a quest to prevent a war between the gods.",
                        ImageUrl = "https://example.com/lightning-thief.jpg"
                    },
                    new Book
                    {
                        Title = "Harry Potter and the Philosopher's Stone",
                        Author = "J.K. Rowling",
                        ISBN = "978-0747532699",
                        PublicationYear = 1997,
                        Publisher = "Bloomsbury",
                        Category = "Fantasy",
                        Price = 15.99m,
                        StockQuantity = 60,
                        Description = "The first book in the Harry Potter series. Harry discovers he's a wizard and begins his magical education at Hogwarts.",
                        ImageUrl = "https://example.com/harry-potter-1.jpg"
                    }
                };

                _context.Books.AddRange(books);
                await _context.SaveChangesAsync();
            }
        }
    }
}
