using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALProject.Migrations
{
    /// <inheritdoc />
    public partial class Tech : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_AspNetUsers_UserId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Drivers_DriverId",
                table: "OrderHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Technicians_TechnicianId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_TechnicianId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "OrderHeaders");

            migrationBuilder.UpdateData(
                table: "ProductCategorys",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 8, 10, 20, 37, 9, 230, DateTimeKind.Local).AddTicks(6116));

            migrationBuilder.UpdateData(
                table: "ProductCategorys",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 8, 10, 20, 37, 9, 230, DateTimeKind.Local).AddTicks(6127));

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_AspNetUsers_UserId",
                table: "Cars",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Drivers_DriverId",
                table: "OrderHeaders",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_AspNetUsers_UserId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Drivers_DriverId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "OrderHeaders",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_TechnicianId",
                table: "OrderHeaders",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_AspNetUsers_UserId",
                table: "Cars",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Drivers_DriverId",
                table: "OrderHeaders",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Technicians_TechnicianId",
                table: "OrderHeaders",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");
        }
    }
}
