namespace UnravelTravel.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ActivityDescriptionAndAdditionInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Activities",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Activities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Activities");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Activities",
                nullable: true,
                oldClrType: typeof(decimal));
        }
    }
}
