using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketServer.Migrations
{
    /// <inheritdoc />
    public partial class SellerObjId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerObjId",
                table: "Market",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerObjId",
                table: "Market");
        }
    }
}
