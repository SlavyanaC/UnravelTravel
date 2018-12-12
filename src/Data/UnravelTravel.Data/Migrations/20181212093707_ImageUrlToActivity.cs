using Microsoft.EntityFrameworkCore.Migrations;

namespace UnravelTravel.Data.Migrations
{
    public partial class ImageUrlToActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Activities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Activities");
        }
    }
}
