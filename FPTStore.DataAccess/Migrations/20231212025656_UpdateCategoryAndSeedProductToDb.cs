using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FPTStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryAndSeedProductToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductAuthor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "DisplayOrder" },
                values: new object[] { 4, "IT", 4 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ISBN", "ListPrice", "Price", "Price100", "Price50", "ProductAuthor", "ProductDescription", "ProductTitle" },
                values: new object[,]
                {
                    { 1, "SWD9999001", 99.0, 90.0, 80.0, 85.0, "Billy Spark", "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ", "Fortune of Time" },
                    { 2, "CAW777777701", 40.0, 30.0, 20.0, 25.0, "Nancy Hoover", "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ", "Dark Skies" },
                    { 3, "RITO5555501", 55.0, 50.0, 35.0, 40.0, "Julian Button", "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ", "Vanish in the Sunset" },
                    { 4, "WS3333333301", 70.0, 65.0, 55.0, 60.0, "Abby Muscles", "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ", "Cotton Candy" },
                    { 5, "SOTJ1111111101", 30.0, 27.0, 20.0, 25.0, "Ron Parker", "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ", "Rock in the Ocean" },
                    { 6, "FOT000000001", 25.0, 23.0, 20.0, 22.0, "Laura Phantom", "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ", "Leaves and Wonders" },
                    { 7, "EE999888801", 60.0, 55.0, 45.0, 50.0, "Lily Harmony", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.", "Eternal Echoes" },
                    { 8, "WITW777777701", 45.0, 40.0, 30.0, 35.0, "Owen Zephyr", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.", "Whispers in the Wind" },
                    { 9, "MM111122223301", 75.0, 70.0, 60.0, 65.0, "Aria Moon", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.", "Mystical Meadows" },
                    { 10, "SSSS888888801", 35.0, 32.0, 28.0, 30.0, "Ethan Sunshine", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.", "Sunset Serenity" },
                    { 11, "ITA111122223301", 90.0, 85.0, 75.0, 80.0, "Thomas H. Cormen", "A comprehensive introduction to algorithms and data structures.", "Introduction to Algorithms" },
                    { 12, "CCASH777777701", 55.0, 50.0, 40.0, 45.0, "Robert C. Martin", "Guidelines for writing clean, maintainable, and efficient code.", "Clean Code: A Handbook of Agile Software Craftsmanship" },
                    { 13, "PRAGMA3333333301", 70.0, 65.0, 55.0, 60.0, "David Thomas, Andrew Hunt", "Practical advice on software development and becoming an effective programmer.", "The Pragmatic Programmer: Your Journey to Mastery" },
                    { 14, "DPATTERN1111111101", 40.0, 35.0, 25.0, 30.0, "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides", "Classic book on object-oriented design patterns.", "Design Patterns: Elements of Reusable Object-Oriented Software" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);
        }
    }
}
