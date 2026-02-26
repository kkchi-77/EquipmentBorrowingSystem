using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "tMember",
                columns: table => new
                {
                    fId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fUserId = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    fPwd = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMember", x => x.fId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tMember");
        }
    }
}
