using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookSchemaWithNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Books",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Books_Name",
                table: "Books",
                newName: "IX_Books_Title");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "Books",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PublicationYear",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Books",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Author", "ISBN", "ImageUrl", "PublicationYear", "Publisher", "StockQuantity" },
                values: new object[] { "F. Scott Fitzgerald", "978-0743273565", "", 1925, "Scribner", 50 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Author", "ISBN", "ImageUrl", "PublicationYear", "Publisher", "StockQuantity" },
                values: new object[] { "Harper Lee", "978-0446310789", "", 1960, "J.B. Lippincott & Co.", 30 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Author", "ISBN", "ImageUrl", "PublicationYear", "Publisher", "StockQuantity" },
                values: new object[] { "George Orwell", "978-0451524935", "", 1949, "Secker & Warburg", 45 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Author", "ISBN", "ImageUrl", "PublicationYear", "Publisher", "StockQuantity" },
                values: new object[] { "Robert C. Martin", "978-0132350884", "", 2008, "Prentice Hall", 25 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Author", "ISBN", "ImageUrl", "PublicationYear", "Publisher", "StockQuantity" },
                values: new object[] { "Gang of Four", "978-0201633610", "", 1994, "Addison-Wesley", 15 });

            migrationBuilder.CreateIndex(
                name: "IX_Books_Author",
                table: "Books",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Author",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_ISBN",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublicationYear",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Books",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Books_Title",
                table: "Books",
                newName: "IX_Books_Name");
        }
    }
}
