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
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        ISBN = "978-0132350884",
                        PublicationYear = 2008,
                        Publisher = "Prentice Hall",
                        Category = "Programming",
                        Price = 45.99m,
                        StockQuantity = 25,
                        Description = "A Handbook of Agile Software Craftsmanship by Robert C. Martin",
                        ImageUrl = "https://example.com/clean-code.jpg"
                    },
                    new Book
                    {
                        Title = "Design Patterns",
                        Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                        ISBN = "978-0201633610",
                        PublicationYear = 1994,
                        Publisher = "Addison-Wesley Professional",
                        Category = "Programming",
                        Price = 54.99m,
                        StockQuantity = 15,
                        Description = "Elements of Reusable Object-Oriented Software",
                        ImageUrl = "https://example.com/design-patterns.jpg"
                    },
                    new Book
                    {
                        Title = "The Pragmatic Programmer",
                        Author = "David Thomas, Andrew Hunt",
                        ISBN = "978-0135957059",
                        PublicationYear = 2019,
                        Publisher = "Addison-Wesley Professional",
                        Category = "Programming",
                        Price = 49.99m,
                        StockQuantity = 30,
                        Description = "Your Journey to Mastery by David Thomas and Andrew Hunt",
                        ImageUrl = "https://example.com/pragmatic-programmer.jpg"
                    },
                    new Book
                    {
                        Title = "Effective C#",
                        Author = "Bill Wagner",
                        ISBN = "978-0672337871",
                        PublicationYear = 2016,
                        Publisher = "Addison-Wesley Professional",
                        Category = "Programming",
                        Price = 42.99m,
                        StockQuantity = 20,
                        Description = "50 Specific Ways to Improve Your C# by Bill Wagner",
                        ImageUrl = "https://example.com/effective-csharp.jpg"
                    },
                    new Book
                    {
                        Title = "You Don't Know JS",
                        Author = "Kyle Simpson",
                        ISBN = "978-1491924464",
                        PublicationYear = 2015,
                        Publisher = "O'Reilly Media",
                        Category = "Web Development",
                        Price = 39.99m,
                        StockQuantity = 35,
                        Description = "A book series on JavaScript by Kyle Simpson",
                        ImageUrl = "https://example.com/you-dont-know-js.jpg"
                    },
                    new Book
                    {
                        Title = "React Up & Running",
                        Author = "Stoyan Stefanov",
                        ISBN = "978-1491931820",
                        PublicationYear = 2016,
                        Publisher = "O'Reilly Media",
                        Category = "Web Development",
                        Price = 41.99m,
                        StockQuantity = 22,
                        Description = "Building Web Applications by Stoyan Stefanov",
                        ImageUrl = "https://example.com/react-up-running.jpg"
                    },
                    new Book
                    {
                        Title = "Node.js Design Patterns",
                        Author = "Mario Casciaro, Luciano Mammino",
                        ISBN = "978-1783287314",
                        PublicationYear = 2020,
                        Publisher = "Packt Publishing",
                        Category = "Web Development",
                        Price = 47.99m,
                        StockQuantity = 18,
                        Description = "Design and implement production-grade Node.js applications",
                        ImageUrl = "https://example.com/nodejs-design-patterns.jpg"
                    },
                    new Book
                    {
                        Title = "Database Design for Mere Mortals",
                        Author = "Michael J. Hernandez",
                        ISBN = "978-0321884497",
                        PublicationYear = 2013,
                        Publisher = "Addison-Wesley Professional",
                        Category = "Database",
                        Price = 52.99m,
                        StockQuantity = 12,
                        Description = "A Hands-On Guide to Relational Database Design",
                        ImageUrl = "https://example.com/database-design.jpg"
                    },
                    new Book
                    {
                        Title = "SQL Performance Explained",
                        Author = "Markus Winand",
                        ISBN = "978-3950307825",
                        PublicationYear = 2012,
                        Publisher = "Markus Winand",
                        Category = "Database",
                        Price = 38.99m,
                        StockQuantity = 28,
                        Description = "Everything Developers Need to Know about SQL Performance",
                        ImageUrl = "https://example.com/sql-performance.jpg"
                    },
                    new Book
                    {
                        Title = "System Design Interview",
                        Author = "Alex Xu",
                        ISBN = "978-1736049129",
                        PublicationYear = 2020,
                        Publisher = "Independently published",
                        Category = "System Design",
                        Price = 44.99m,
                        StockQuantity = 40,
                        Description = "An insider's guide by Alex Xu",
                        ImageUrl = "https://example.com/system-design-interview.jpg"
                    }
                };

                _context.Books.AddRange(books);
                await _context.SaveChangesAsync();
            }
        }
    }
}
