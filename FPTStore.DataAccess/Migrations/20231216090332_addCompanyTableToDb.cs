using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCompanyTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companys",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyStreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companys", x => x.CompanyId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companys");
        }
    }
}
