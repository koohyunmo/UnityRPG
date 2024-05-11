using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class ItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuickSlot",
                columns: table => new
                {
                    QuickSlotDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerDbId = table.Column<int>(type: "int", nullable: false),
                    SlotNumber = table.Column<int>(type: "int", nullable: false),
                    ItemDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickSlot", x => x.QuickSlotDbId);
                    table.ForeignKey(
                        name: "FK_QuickSlot_Item_ItemDbId",
                        column: x => x.ItemDbId,
                        principalTable: "Item",
                        principalColumn: "ItemDbId");
                    table.ForeignKey(
                        name: "FK_QuickSlot_Player_PlayerDbId",
                        column: x => x.PlayerDbId,
                        principalTable: "Player",
                        principalColumn: "PlayerDbId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuickSlot_ItemDbId",
                table: "QuickSlot",
                column: "ItemDbId");

            migrationBuilder.CreateIndex(
                name: "IX_QuickSlot_PlayerDbId",
                table: "QuickSlot",
                column: "PlayerDbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuickSlot");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Item");
        }
    }
}
