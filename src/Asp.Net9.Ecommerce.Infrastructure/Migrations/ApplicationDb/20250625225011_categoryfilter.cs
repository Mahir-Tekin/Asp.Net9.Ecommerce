using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class categoryfilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariationTypes",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "CategoryVariationTypes",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariationTypesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryVariationTypes", x => new { x.CategoriesId, x.VariationTypesId });
                    table.ForeignKey(
                        name: "FK_CategoryVariationTypes_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryVariationTypes_VariationTypes_VariationTypesId",
                        column: x => x.VariationTypesId,
                        principalTable: "VariationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariationTypes_VariationTypesId",
                table: "CategoryVariationTypes",
                column: "VariationTypesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryVariationTypes");

            migrationBuilder.AddColumn<string>(
                name: "VariationTypes",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
