using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALProject.Migrations
{
    /// <inheritdoc />
    public partial class deleteStartEndTimeFromOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "FinalReport",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "OrderHeaders");

            migrationBuilder.UpdateData(
                table: "ProductCategorys",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 8, 8, 22, 59, 37, 331, DateTimeKind.Local).AddTicks(9397));

            migrationBuilder.UpdateData(
                table: "ProductCategorys",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 8, 8, 22, 59, 37, 331, DateTimeKind.Local).AddTicks(9404));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalReport",
                table: "OrderHeaders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ProductCategorys",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 8, 8, 18, 7, 40, 445, DateTimeKind.Local).AddTicks(7945));

            migrationBuilder.UpdateData(
                table: "ProductCategorys",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 8, 8, 18, 7, 40, 445, DateTimeKind.Local).AddTicks(7956));
        }
    }
}
