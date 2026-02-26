using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scaffoldong.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tManager",
                columns: table => new
                {
                    fId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fManagerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fManagerPwd = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tManager", x => x.fId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tManager");
        }
    }
}
