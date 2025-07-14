using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStore.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PublicationYear = table.Column<int>(type: "integer", nullable: false),
                    Publisher = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ShippingAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Category", "CreatedAt", "Description", "ISBN", "ImageUrl", "IsDeleted", "Price", "PublicationYear", "Publisher", "StockQuantity", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, "Jane Austen", "Fiction - Romance", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A classic romance novel exploring societal expectations and true love in 19th century England.", "978-0141439518", "https://covers.openlibrary.org/b/id/8231852-L.jpg", false, 10.99m, 1813, "Penguin Classics", 40, "Pride and Prejudice", null, null },
                    { 2, "John Green", "Fiction - Romance", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Two teenagers with cancer fall in love while navigating life's harsh realities.", "978-0525478812", "https://covers.openlibrary.org/b/id/8231863-L.jpg", false, 12.99m, 2012, "Dutton Books", 35, "The Fault in Our Stars", null, null },
                    { 3, "Stieg Larsson", "Fiction - Thriller", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A journalist and hacker investigate a decades-old disappearance in Sweden.", "978-0307454546", "https://covers.openlibrary.org/b/id/240726-L.jpg", false, 13.99m, 2005, "Norstedts Förlag", 30, "The Girl with the Dragon Tattoo", null, null },
                    { 4, "Gillian Flynn", "Fiction - Thriller", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A psychological thriller about a marriage gone terribly wrong.", "978-0307588371", "https://covers.openlibrary.org/b/id/8235119-L.jpg", false, 14.99m, 2012, "Crown Publishing", 25, "Gone Girl", null, null },
                    { 5, "J.K. Rowling", "Fiction - Fantasy", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The first book in the magical series about a young wizard.", "978-0590353427", "https://covers.openlibrary.org/b/id/7984916-L.jpg", false, 9.99m, 1997, "Scholastic", 50, "Harry Potter and the Sorcerer's Stone", null, null },
                    { 6, "Erin Morgenstern", "Fiction - Fantasy", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magical competition between two illusionists in a mysterious circus.", "978-0307744432", "https://covers.openlibrary.org/b/id/8235120-L.jpg", false, 15.99m, 2011, "Doubleday", 20, "The Night Circus", null, null },
                    { 13, "Walter Isaacson", "Non-Fiction - Biography", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The authorized biography of Apple's co-founder.", "978-1451648539", "https://covers.openlibrary.org/b/id/7265441-L.jpg", false, 18.99m, 2011, "Simon & Schuster", 15, "Steve Jobs", null, null },
                    { 14, "Michelle Obama", "Non-Fiction - Biography", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Memoir of the former First Lady of the United States.", "978-1524763138", "https://covers.openlibrary.org/b/id/8235124-L.jpg", false, 19.99m, 2018, "Crown", 20, "Becoming", null, null },
                    { 15, "James Clear", "Non-Fiction - Self-Help", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A guide to building good habits and breaking bad ones.", "978-0735211292", "https://covers.openlibrary.org/b/id/9259256-L.jpg", false, 16.99m, 2018, "Avery", 25, "Atomic Habits", null, null },
                    { 16, "Yuval Noah Harari", "Non-Fiction - History", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exploration of human history from evolution to modern society.", "978-0062316097", "https://covers.openlibrary.org/b/id/8231857-L.jpg", false, 19.99m, 2011, "Harper", 20, "Sapiens: A Brief History of Humankind", null, null },
                    { 17, "David Grann", "Non-Fiction - History", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The Osage murders and birth of the FBI.", "978-0385534246", "https://covers.openlibrary.org/b/id/8235125-L.jpg", false, 17.99m, 2017, "Doubleday", 15, "Killers of the Flower Moon", null, null },
                    { 18, "Barack Obama", "Non-Fiction - Politics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thoughts on reclaiming the American dream.", "978-0307237705", "https://covers.openlibrary.org/b/id/8231858-L.jpg", false, 14.99m, 2006, "Crown", 18, "The Audacity of Hope", null, null },
                    { 19, "Truman Capote", "Non-Fiction - True Crime", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "True crime classic about the Clutter family murders.", "978-0679745587", "https://covers.openlibrary.org/b/id/8231859-L.jpg", false, 13.99m, 1966, "Vintage", 12, "In Cold Blood", null, null },
                    { 20, "Rick Warren", "Non-Fiction - Religion", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Spiritual guide to finding purpose through faith.", "978-0310337508", "https://covers.openlibrary.org/b/id/8231860-L.jpg", false, 12.99m, 2002, "Zondervan", 14, "The Purpose Driven Life", null, null },
                    { 21, "Eric Carle", "Children & Teens - Picture Books", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A caterpillar eats his way through various foods.", "978-0399226908", "https://covers.openlibrary.org/b/id/8231861-L.jpg", false, 8.99m, 1969, "Philomel Books", 30, "The Very Hungry Caterpillar", null, null },
                    { 22, "Rick Riordan", "Children & Teens - Middle Grade", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A modern demigod discovers his divine heritage.", "978-0786838653", "https://covers.openlibrary.org/b/id/8231862-L.jpg", false, 9.99m, 2005, "Disney-Hyperion", 22, "Percy Jackson & the Olympians: The Lightning Thief", null, null },
                    { 23, "R.J. Palacio", "Children & Teens - Middle Grade", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A boy with facial differences attends mainstream school.", "978-0375869020", "https://covers.openlibrary.org/b/id/8235126-L.jpg", false, 10.99m, 2012, "Knopf", 18, "Wonder", null, null },
                    { 24, "Angie Thomas", "Children & Teens - Young Adult", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A teen witnesses police shooting her childhood friend.", "978-0062498533", "https://covers.openlibrary.org/b/id/8235127-L.jpg", false, 12.99m, 2017, "Balzer + Bray", 20, "The Hate U Give", null, null },
                    { 25, "Veronica Roth", "Children & Teens - Young Adult", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dystopian Chicago where society is divided into factions.", "978-0062024039", "https://covers.openlibrary.org/b/id/8235128-L.jpg", false, 11.99m, 2011, "Katherine Tegen Books", 16, "Divergent", null, null },
                    { 26, "Stephen Hawking", "Academic & Textbooks - Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Introduction to cosmology for non-specialists.", "978-0553380163", "https://covers.openlibrary.org/b/id/8231864-L.jpg", false, 17.99m, 1988, "Bantam", 10, "A Brief History of Time", null, null },
                    { 27, "J.E. Gordon", "Academic & Textbooks - Engineering", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fundamentals of structural engineering made accessible.", "978-0306812837", "https://covers.openlibrary.org/b/id/8231865-L.jpg", false, 15.99m, 1978, "Da Capo Press", 8, "Structures: Or Why Things Don't Fall Down", null, null },
                    { 28, "Siddhartha Mukherjee", "Academic & Textbooks - Medicine", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Biography of cancer spanning centuries of research.", "978-1439170915", "https://covers.openlibrary.org/b/id/8231866-L.jpg", false, 18.99m, 2010, "Scribner", 7, "The Emperor of All Maladies", null, null },
                    { 29, "Harper Lee", "Academic & Textbooks - Law", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exploration of racial injustice and moral growth.", "978-0060935467", "https://covers.openlibrary.org/b/id/8228692-L.jpg", false, 14.99m, 1960, "J.B. Lippincott & Co.", 9, "To Kill a Mockingbird", null, null },
                    { 30, "The Princeton Review", "Academic & Textbooks - Test Prep", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comprehensive SAT preparation guide with practice tests.", "978-0525568063", "https://covers.openlibrary.org/b/id/8231867-L.jpg", false, 21.99m, 2020, "Princeton Review", 12, "Cracking the SAT", null, null },
                    { 31, "Masashi Kishimoto", "Comics & Graphic Novels - Manga", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A young ninja dreams of becoming his village's leader.", "978-1569319000", "https://covers.openlibrary.org/b/id/8231868-L.jpg", false, 9.99m, 2003, "VIZ Media", 20, "Naruto, Vol. 1", null, null },
                    { 32, "Frank Miller", "Comics & Graphic Novels - Superhero", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Batman's early crime-fighting days in Gotham City.", "978-1401207526", "https://covers.openlibrary.org/b/id/8231869-L.jpg", false, 12.99m, 1987, "DC Comics", 15, "Batman: Year One", null, null },
                    { 33, "Marjane Satrapi", "Comics & Graphic Novels - Indie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Graphic memoir of growing up during Iranian Revolution.", "978-0375714573", "https://covers.openlibrary.org/b/id/8231870-L.jpg", false, 13.99m, 2000, "Pantheon", 10, "Persepolis", null, null },
                    { 34, "Rachel Smythe", "Comics & Graphic Novels - Webtoons", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern retelling of Hades and Persephone myth.", "978-0593160290", "https://covers.openlibrary.org/b/id/8231871-L.jpg", false, 16.99m, 2021, "Del Rey", 8, "Lore Olympus: Volume One", null, null },
                    { 35, "Samin Nosrat", "Lifestyle & Hobbies - Cooking", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mastering the elements of good cooking.", "978-1476753836", "https://covers.openlibrary.org/b/id/8231872-L.jpg", false, 24.99m, 2017, "Simon & Schuster", 10, "Salt, Fat, Acid, Heat", null, null },
                    { 36, "Lonely Planet", "Lifestyle & Hobbies - Travel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ranking of the world's top travel experiences.", "978-1788689199", "https://covers.openlibrary.org/b/id/8231873-L.jpg", false, 29.99m, 2020, "Lonely Planet", 7, "Lonely Planet's Ultimate Travel List", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_Author",
                table: "Books",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Category",
                table: "Books",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN");

            migrationBuilder.CreateIndex(
                name: "IX_Books_IsDeleted",
                table: "Books",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Books_UserId",
                table: "Books",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_BookId",
                table: "CartItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_BookId",
                table: "CartItems",
                columns: new[] { "UserId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BookId",
                table: "OrderItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_BookId",
                table: "UserFavorites",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId_BookId",
                table: "UserFavorites",
                columns: new[] { "UserId", "BookId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
