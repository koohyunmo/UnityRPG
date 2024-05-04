using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class mail3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mail_Player_OnwerDbId",
                table: "Mail");

            migrationBuilder.RenameColumn(
                name: "OnwerDbId",
                table: "Mail",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Mail_OnwerDbId",
                table: "Mail",
                newName: "IX_Mail_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mail_Player_OwnerId",
                table: "Mail",
                column: "OwnerId",
                principalTable: "Player",
                principalColumn: "PlayerDbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mail_Player_OwnerId",
                table: "Mail");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Mail",
                newName: "OnwerDbId");

            migrationBuilder.RenameIndex(
                name: "IX_Mail_OwnerId",
                table: "Mail",
                newName: "IX_Mail_OnwerDbId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mail_Player_OnwerDbId",
                table: "Mail",
                column: "OnwerDbId",
                principalTable: "Player",
                principalColumn: "PlayerDbId");
        }
    }
}
