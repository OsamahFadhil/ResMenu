using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Menufy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuDesignSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDesignOnly",
                table: "MenuTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LayoutConfiguration",
                table: "MenuTemplates",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuDesigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LayoutConfiguration = table.Column<string>(type: "jsonb", nullable: false),
                    GlobalTheme = table.Column<string>(type: "jsonb", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuDesigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuDesigns_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuDesigns_CreatedAt",
                table: "MenuDesigns",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MenuDesigns_RestaurantId_IsActive",
                table: "MenuDesigns",
                columns: new[] { "RestaurantId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuDesigns");

            migrationBuilder.DropColumn(
                name: "IsDesignOnly",
                table: "MenuTemplates");

            migrationBuilder.DropColumn(
                name: "LayoutConfiguration",
                table: "MenuTemplates");
        }
    }
}
