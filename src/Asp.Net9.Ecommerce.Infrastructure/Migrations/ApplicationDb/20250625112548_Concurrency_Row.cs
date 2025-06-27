using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class Concurrency_Row : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ProductVariants",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProductVariants");
        }
    }
}
