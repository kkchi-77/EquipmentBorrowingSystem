using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowingSystem.Migrations
{
    /// <inheritdoc />
    public partial class equipment1 : Migration
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
                    Emodel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EQuantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EBorrowing_Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ERemaining_Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMissing_quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EQuantity_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ESource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Is_Consumable = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
