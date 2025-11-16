using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Menufy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class menudesignheadersettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderColor",
                table: "MenuDesigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeaderDisplayMode",
                table: "MenuDesigns",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderImageUrl",
                table: "MenuDesigns",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderColor",
                table: "MenuDesigns");

            migrationBuilder.DropColumn(
                name: "HeaderDisplayMode",
                table: "MenuDesigns");

            migrationBuilder.DropColumn(
                name: "HeaderImageUrl",
                table: "MenuDesigns");
        }
    }
}
