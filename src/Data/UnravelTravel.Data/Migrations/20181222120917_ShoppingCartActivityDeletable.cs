using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UnravelTravel.Data.Migrations
{
    public partial class ShoppingCartActivityDeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ShoppingCartActivities",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShoppingCartActivities",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartActivities_IsDeleted",
                table: "ShoppingCartActivities",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartActivities_IsDeleted",
                table: "ShoppingCartActivities");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ShoppingCartActivities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShoppingCartActivities");
        }
    }
}
