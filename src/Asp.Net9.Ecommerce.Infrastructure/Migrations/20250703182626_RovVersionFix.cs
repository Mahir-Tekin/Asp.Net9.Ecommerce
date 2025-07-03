using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RovVersionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "ProductVariants",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "'\\x00000001'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "ProductVariants",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "'\\x00000001'::bytea");
        }
    }
}
