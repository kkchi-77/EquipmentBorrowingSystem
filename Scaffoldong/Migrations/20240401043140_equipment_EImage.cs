using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scaffoldong.Migrations
{
    /// <inheritdoc />
    public partial class equipment_EImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EImage",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EImage",
                table: "Equipment");
        }
    }
}
