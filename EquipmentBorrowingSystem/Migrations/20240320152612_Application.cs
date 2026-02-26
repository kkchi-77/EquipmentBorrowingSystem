using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowingSystem.Migrations
{
    /// <inheritdoc />
    public partial class Application : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    fId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date_Of_Application = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fOrderGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Borrow_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Return_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Illustrate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.fId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");
        }
    }
}
