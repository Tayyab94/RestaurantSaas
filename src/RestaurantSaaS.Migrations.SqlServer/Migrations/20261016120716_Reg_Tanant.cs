using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSaaS.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Reg_Tanant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email",
                table: "Tenants",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Phone",
                table: "Tenants",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_Email",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Phone",
                table: "Tenants");
        }
    }
}
