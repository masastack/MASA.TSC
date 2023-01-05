using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc010502 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PanelMetric_Panel_PanelId",
                schema: "tsc",
                table: "PanelMetric");

            migrationBuilder.DropIndex(
                name: "IX_PanelMetric_PanelId",
                schema: "tsc",
                table: "PanelMetric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PanelMetric_PanelId",
                schema: "tsc",
                table: "PanelMetric",
                column: "PanelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PanelMetric_Panel_PanelId",
                schema: "tsc",
                table: "PanelMetric",
                column: "PanelId",
                principalSchema: "tsc",
                principalTable: "Panel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
