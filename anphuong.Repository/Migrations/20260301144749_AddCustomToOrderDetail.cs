using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace anphuong.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomToOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CustomHeightSize",
                table: "OrderDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomLongSize",
                table: "OrderDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomWidthSize",
                table: "OrderDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isCustomized",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomHeightSize",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CustomLongSize",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CustomWidthSize",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "isCustomized",
                table: "OrderDetails");
        }
    }
}
