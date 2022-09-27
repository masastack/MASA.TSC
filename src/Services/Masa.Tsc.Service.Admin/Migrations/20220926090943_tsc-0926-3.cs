using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc09263 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Panel_Panel_PanelId",
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

            migrationBuilder.RenameColumn(
                name: "Index",
                schema: "tsc",
                table: "Panel",
                newName: "Sort");

            migrationBuilder.AddColumn<string>(
                name: "Height",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Width",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Instrument_DirectoryId",
                schema: "tsc",
                table: "Instrument",
                column: "DirectoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instrument_Directory_DirectoryId",
                schema: "tsc",
                table: "Instrument",
                column: "DirectoryId",
                principalSchema: "tsc",
                principalTable: "Directory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instrument_Directory_DirectoryId",
                schema: "tsc",
                table: "Instrument");

            migrationBuilder.DropIndex(
                name: "IX_Instrument_DirectoryId",
                schema: "tsc",
                table: "Instrument");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.RenameColumn(
                name: "Sort",
                schema: "tsc",
                table: "Panel",
                newName: "Index");

            migrationBuilder.AddColumn<Guid>(
                name: "PanelId",
                schema: "tsc",
                table: "Panel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Panel_PanelId",
                schema: "tsc",
                table: "Panel",
                column: "PanelId");

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
