using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scaffoldong.Migrations
{
    /// <inheritdoc />
    public partial class Application_Completed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application_Completed",
                columns: table => new
                {
                    fId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date_Of_Application = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fOrderGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Borrow_Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Return_Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Illustrate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credentials_Mortgage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Equipment_Handover_Person = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Equipment_Recive_Person = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSendEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Completed", x => x.fId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application_Completed");
        }
    }
}
