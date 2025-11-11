using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Menufy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Structure = table.Column<string>(type: "jsonb", nullable: false),
                    Theme = table.Column<string>(type: "jsonb", nullable: true),
                    Tags = table.Column<string>(type: "jsonb", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuTemplates_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuTemplates_RestaurantId_Name",
                table: "MenuTemplates",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuTemplates");
        }
    }
}
