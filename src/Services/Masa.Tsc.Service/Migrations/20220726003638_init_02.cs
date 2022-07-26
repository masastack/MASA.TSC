using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class init_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Interval",
                schema: "tsc",
                table: "Setting",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Interval",
                schema: "tsc",
                table: "Setting",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }
    }
}
