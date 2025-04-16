using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class pending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("308660dc-ae51-480f-824d-7dca6714c3e2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 13, 22, 51, 54, 331, DateTimeKind.Utc).AddTicks(4938));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7be43da-622c-4cfe-98a9-5a5161120d24"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 13, 22, 51, 54, 342, DateTimeKind.Utc).AddTicks(1571));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("308660dc-ae51-480f-824d-7dca6714c3e2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 13, 22, 51, 22, 668, DateTimeKind.Utc).AddTicks(2749));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7be43da-622c-4cfe-98a9-5a5161120d24"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 13, 22, 51, 22, 678, DateTimeKind.Utc).AddTicks(5938));
        }
    }
}
