using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace anphuong.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailImages_Products_ProductId1",
                table: "DetailImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Variants_VariantId1",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_VariantId1",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_DetailImages_ProductId1",
                table: "DetailImages");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VariantId1",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "DetailImages");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VariantId1",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "DetailImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_VariantId1",
                table: "Inventories",
                column: "VariantId1",
                unique: true,
                filter: "[VariantId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetailImages_ProductId1",
                table: "DetailImages",
                column: "ProductId1",
                unique: true,
                filter: "[ProductId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId1",
                table: "Customers",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailImages_Products_ProductId1",
                table: "DetailImages",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Variants_VariantId1",
                table: "Inventories",
                column: "VariantId1",
                principalTable: "Variants",
                principalColumn: "Id");
        }
    }
}
