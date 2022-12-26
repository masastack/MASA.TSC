using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc_1226 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartType",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropColumn(
                name: "UiType",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "tsc",
                table: "PanelMetric",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Width",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Top",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Left",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Height",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ExtensionData",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Icon",
                schema: "tsc",
                table: "PanelMetric");

            migrationBuilder.DropColumn(
                name: "ExtensionData",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.DropColumn(
                name: "PanelId",
                schema: "tsc",
                table: "Panel");

            migrationBuilder.AlterColumn<string>(
                name: "Width",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Top",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Left",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Height",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<string>(
                name: "ChartType",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UiType",
                schema: "tsc",
                table: "Panel",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
