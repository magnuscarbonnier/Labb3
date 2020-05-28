using Microsoft.EntityFrameworkCore.Migrations;

namespace OrdersAPI.Migrations
{
    public partial class itemsinorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ImgSrc",
                table: "OrderItem");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgSrc",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
