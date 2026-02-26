using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application_Details",
                columns: table => new
                {
                    fId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),               
                    fOrderGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ESource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fIsApplied = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Details", x => x.fId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application_Details");
        }
    }
}
