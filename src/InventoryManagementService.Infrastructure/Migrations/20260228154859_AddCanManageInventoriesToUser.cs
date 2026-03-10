using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCanManageInventoriesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanManageInventories",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanManageInventories",
                table: "AspNetUsers");
        }
    }
}
