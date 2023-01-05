using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc0105 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Panel_Instrument_InstrumentId",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropForeignKey(
                name: "FK_Panel_Panel_PanelId",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropIndex(
                name: "IX_Panel_InstrumentId",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropIndex(
                name: "IX_Panel_PanelId",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropColumn(
                name: "PanelId",
                schema: "tsc",
                table: "Panel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PanelId",
                schema: "tsc",
                table: "Panel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Panel_InstrumentId",
                schema: "tsc",
                table: "Panel",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Panel_PanelId",
                schema: "tsc",
                table: "Panel",
                column: "PanelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Panel_Instrument_InstrumentId",
                schema: "tsc",
                table: "Panel",
                column: "InstrumentId",
                principalSchema: "tsc",
                principalTable: "Instrument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Panel_Panel_PanelId",
                schema: "tsc",
                table: "Panel",
                column: "PanelId",
                principalSchema: "tsc",
                principalTable: "Panel",
                principalColumn: "Id");
        }
    }
}
