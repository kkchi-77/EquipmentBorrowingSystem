using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scaffoldong.Migrations
{
    /// <inheritdoc />
    public partial class equipment_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Consumable_Borrowing_Quantity",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Is_Consumable",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consumable_Borrowing_Quantity",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Is_Consumable",
                table: "Equipment");
        }
    }
}
