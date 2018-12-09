using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UnravelTravel.Data.Migrations
{
    public partial class RemoveTicketCompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tickets",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Tickets",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Tickets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Tickets",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tickets",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                columns: new[] { "UserId", "ActivityId" });
        }
    }
}
