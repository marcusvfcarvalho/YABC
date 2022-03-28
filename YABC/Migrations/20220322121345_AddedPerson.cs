using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YABC.Migrations
{
    public partial class AddedPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Block",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Block",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Block_PersonId",
                table: "Block",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Person_PersonId",
                table: "Block",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Person_PersonId",
                table: "Block");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Block_PersonId",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Block");
        }
    }
}
