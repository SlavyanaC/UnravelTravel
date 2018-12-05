using Microsoft.EntityFrameworkCore.Migrations;

namespace UnravelTravel.Data.Migrations
{
    public partial class CountryDestinationRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Destinations");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Destinations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_CountryId",
                table: "Destinations",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Destinations_Countries_CountryId",
                table: "Destinations",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Destinations_Countries_CountryId",
                table: "Destinations");

            migrationBuilder.DropIndex(
                name: "IX_Destinations_CountryId",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Destinations");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Destinations",
                nullable: true);
        }
    }
}
