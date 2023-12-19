using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fixOrderHeaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShoppingDate",
                table: "OrderHeaders",
                newName: "ShippingDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingDate",
                table: "OrderHeaders",
                newName: "ShoppingDate");
        }
    }
}
