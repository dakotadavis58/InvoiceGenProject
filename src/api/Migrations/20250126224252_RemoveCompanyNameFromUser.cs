using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceGeneratorAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompanyNameFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId1",
                table: "Companies",
                column: "UserId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_UserId1",
                table: "Companies",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_UserId1",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId1",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }
    }
}
