using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scaffoldong.Migrations
{
    /// <inheritdoc />
    public partial class equipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emodel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EQuantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EBorrowing_Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ERemaining_Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMissing_quantity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EQuantity_Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ESource = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipment_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emodel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ESource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBorrow = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ECurrent_Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAddEquipment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment_Details", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Equipment_Details");
        }
    }
}
