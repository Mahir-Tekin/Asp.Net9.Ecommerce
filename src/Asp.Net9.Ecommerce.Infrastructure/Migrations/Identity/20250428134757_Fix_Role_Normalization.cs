using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations.Identity
{
    /// <inheritdoc />
    public partial class Fix_Role_Normalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("308660dc-ae51-480f-824d-7dca6714c3e2"),
                column: "NormalizedName",
                value: "ADMIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("308660dc-ae51-480f-824d-7dca6714c3e2"),
                column: "NormalizedName",
                value: "ADMİN");
        }
    }
}
