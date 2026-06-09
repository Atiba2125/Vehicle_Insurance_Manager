using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleShield.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimsToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Claims",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CustomerId",
                table: "Claims",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Customers_CustomerId",
                table: "Claims",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Customers_CustomerId",
                table: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_Claims_CustomerId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Claims");
        }
    }
}
