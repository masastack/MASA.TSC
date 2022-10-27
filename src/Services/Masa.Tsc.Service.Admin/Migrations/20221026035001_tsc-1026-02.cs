using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc102602 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                schema: "tsc",
                table: "PanelMetric",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                schema: "tsc",
                table: "PanelMetric");
        }
    }
}
