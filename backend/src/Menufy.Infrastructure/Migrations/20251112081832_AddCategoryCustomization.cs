using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Menufy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryCustomization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomLayout",
                table: "MenuCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomTheme",
                table: "MenuCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "MenuCategories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomLayout",
                table: "MenuCategories");

            migrationBuilder.DropColumn(
                name: "CustomTheme",
                table: "MenuCategories");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "MenuCategories");
        }
    }
}
