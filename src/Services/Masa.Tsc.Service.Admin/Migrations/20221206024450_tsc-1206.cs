using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc1206 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                schema: "tsc",
                table: "Directory");

            migrationBuilder.RenameColumn(
                name: "Sort",
                schema: "tsc",
                table: "Panel",
                newName: "Index");

            migrationBuilder.AddColumn<string>(
                name: "Left",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Top",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lable",
                schema: "tsc",
                table: "Instrument",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Directory_Name",
                schema: "tsc",
                table: "Directory",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Directory_Name",
                schema: "tsc",
                table: "Directory");

            migrationBuilder.DropColumn(
                name: "Left",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropColumn(
                name: "Top",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropColumn(
                name: "Lable",
                schema: "tsc",
                table: "Instrument");

            migrationBuilder.RenameColumn(
                name: "Index",
                schema: "tsc",
                table: "Panel",
                newName: "Sort");

            migrationBuilder.AddColumn<int>(
                name: "Sort",
                schema: "tsc",
                table: "Directory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
