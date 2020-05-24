using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace OrdersAPI.Migrations
{
    public partial class spelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProdutId",
                table: "OrderItem");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "OrderItem",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItem");

            migrationBuilder.AddColumn<Guid>(
                name: "ProdutId",
                table: "OrderItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
