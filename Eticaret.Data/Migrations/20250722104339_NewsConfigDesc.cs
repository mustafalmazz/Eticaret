using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewsConfigDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 7, 22, 13, 43, 38, 872, DateTimeKind.Local).AddTicks(1030), new Guid("a99d3c34-864e-422a-81f0-580d5472eb33") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 7, 22, 13, 43, 38, 874, DateTimeKind.Local).AddTicks(6319));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 7, 22, 13, 43, 38, 874, DateTimeKind.Local).AddTicks(6949));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 7, 22, 13, 37, 8, 292, DateTimeKind.Local).AddTicks(5304), new Guid("373934ac-946e-4d27-a44b-3e278c0a3db6") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 7, 22, 13, 37, 8, 295, DateTimeKind.Local).AddTicks(3005));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 7, 22, 13, 37, 8, 295, DateTimeKind.Local).AddTicks(3756));
        }
    }
}
