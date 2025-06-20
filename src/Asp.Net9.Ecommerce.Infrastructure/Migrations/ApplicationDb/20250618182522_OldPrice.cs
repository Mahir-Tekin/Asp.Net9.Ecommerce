using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class OldPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "ProductVariants");
           
            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
