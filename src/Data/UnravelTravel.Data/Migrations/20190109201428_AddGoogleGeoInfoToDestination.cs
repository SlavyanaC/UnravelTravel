using Microsoft.EntityFrameworkCore.Migrations;

namespace UnravelTravel.Data.Migrations
{
    public partial class AddGoogleGeoInfoToDestination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Destinations",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Destinations",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UtcRawOffset",
                table: "Destinations",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "UtcRawOffset",
                table: "Destinations");
        }
    }
}
