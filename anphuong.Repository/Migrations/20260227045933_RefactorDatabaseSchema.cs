using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace anphuong.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Products_ProductId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_DetailImages_DetailImageId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Variants_VariationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DetailImageId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_VariationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DetailImageId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Material",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VariationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomizeHeight",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CustomizeLong",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CustomizeMaterial",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CustomizeWidth",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "IsCustomize",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "DetailImages");

            migrationBuilder.RenameColumn(
                name: "SubTotalPrice",
                table: "OrderDetails",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderDetails",
                newName: "VariantId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_VariantId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Inventories",
                newName: "VariantId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_ProductId",
                table: "Inventories",
                newName: "IX_Inventories_VariantId");

            migrationBuilder.RenameColumn(
                name: "CustomerAddress",
                table: "Customers",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "ViewCount",
                table: "Behaviors",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "BuyCount",
                table: "Behaviors",
                newName: "CustomerId");

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "Variants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Variants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "Role",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "WidthSize",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "LongSize",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "HeightSize",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "isCustomize",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                table: "OrderDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VariantId1",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image4",
                table: "DetailImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Image3",
                table: "DetailImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Image2",
                table: "DetailImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Image1",
                table: "DetailImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "DetailImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "DetailImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "Behaviors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Behaviors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Variants_MaterialId",
                table: "Variants",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId",
                table: "Variants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_VariantId1",
                table: "Inventories",
                column: "VariantId1",
                unique: true,
                filter: "[VariantId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetailImages_ProductId",
                table: "DetailImages",
                column: "ProductId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Behaviors_CustomerId",
                table: "Behaviors",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Behaviors_ProductId",
                table: "Behaviors",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Behaviors_Customers_CustomerId",
                table: "Behaviors",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Behaviors_Products_ProductId",
                table: "Behaviors",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailImages_Products_ProductId",
                table: "DetailImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetailImages_Products_ProductId1",
                table: "DetailImages",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Variants_VariantId",
                table: "Inventories",
                column: "VariantId",
                principalTable: "Variants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Variants_VariantId1",
                table: "Inventories",
                column: "VariantId1",
                principalTable: "Variants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Variants_VariantId",
                table: "OrderDetails",
                column: "VariantId",
                principalTable: "Variants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_Materials_MaterialId",
                table: "Variants",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Behaviors_Customers_CustomerId",
                table: "Behaviors");

            migrationBuilder.DropForeignKey(
                name: "FK_Behaviors_Products_ProductId",
                table: "Behaviors");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailImages_Products_ProductId",
                table: "DetailImages");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailImages_Products_ProductId1",
                table: "DetailImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Variants_VariantId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Variants_VariantId1",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Variants_VariantId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Variants_Materials_MaterialId",
                table: "Variants");

            migrationBuilder.DropForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Variants_MaterialId",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Variants_ProductId",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_VariantId1",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_DetailImages_ProductId",
                table: "DetailImages");

            migrationBuilder.DropIndex(
                name: "IX_DetailImages_ProductId1",
                table: "DetailImages");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId1",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Behaviors_CustomerId",
                table: "Behaviors");

            migrationBuilder.DropIndex(
                name: "IX_Behaviors_ProductId",
                table: "Behaviors");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isCustomize",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "VariantId1",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DetailImages");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "DetailImages");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "Behaviors");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Behaviors");

            migrationBuilder.RenameColumn(
                name: "VariantId",
                table: "OrderDetails",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderDetails",
                newName: "SubTotalPrice");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_VariantId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId");

            migrationBuilder.RenameColumn(
                name: "VariantId",
                table: "Inventories",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_VariantId",
                table: "Inventories",
                newName: "IX_Inventories_ProductId");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Customers",
                newName: "CustomerAddress");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Behaviors",
                newName: "ViewCount");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Behaviors",
                newName: "BuyCount");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "WidthSize",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "LongSize",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "HeightSize",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "DetailImageId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VariationId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomizeHeight",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomizeLong",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomizeMaterial",
                table: "OrderDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomizeWidth",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomize",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Image4",
                table: "DetailImages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Image3",
                table: "DetailImages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Image2",
                table: "DetailImages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Image1",
                table: "DetailImages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "DetailImages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DetailImageId",
                table: "Products",
                column: "DetailImageId",
                unique: true,
                filter: "[DetailImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_VariationId",
                table: "Products",
                column: "VariationId",
                unique: true,
                filter: "[VariationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Products_ProductId",
                table: "Inventories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_DetailImages_DetailImageId",
                table: "Products",
                column: "DetailImageId",
                principalTable: "DetailImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Variants_VariationId",
                table: "Products",
                column: "VariationId",
                principalTable: "Variants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
