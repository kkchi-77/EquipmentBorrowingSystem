using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowingSystem.Migrations
{
    /// <inheritdoc />
    public partial class Application_Details_IsCount_Borrowing_Times : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsCount_Borrowing_Times",
                table: "Application_Details",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCount_Borrowing_Times",
                table: "Application_Details");
        }
    }
}
