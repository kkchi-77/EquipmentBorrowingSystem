using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowingSystem.Migrations
{
    /// <inheritdoc />
    public partial class IsSendEmail_OverdueNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IsSendEmail_OverdueNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Application_Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSendEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsSendEmail_OverdueNotification", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IsSendEmail_OverdueNotification");
        }
    }
}
