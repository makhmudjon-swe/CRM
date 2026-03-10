using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublicToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Inventories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Inventories");
        }
    }
}
