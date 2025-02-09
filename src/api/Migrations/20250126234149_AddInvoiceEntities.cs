using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceGeneratorAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_UserId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Invoices",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_UserId",
                table: "Invoices",
                newName: "IX_Invoices_CompanyId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentInstructions",
                table: "Invoices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "Invoices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                table: "Invoices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentInstructions",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Invoices",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_CompanyId",
                table: "Invoices",
                newName: "IX_Invoices_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_UserId",
                table: "Invoices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
