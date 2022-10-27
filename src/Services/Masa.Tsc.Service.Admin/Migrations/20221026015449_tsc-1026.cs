using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc1026 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "tsc",
                table: "PanelMetric");

            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "tsc",
                table: "PanelMetric",
                newName: "Caculate");

            migrationBuilder.RenameIndex(
                name: "index_state_timessent_modificationtime",
                schema: "tsc",
                table: "IntegrationEventLog",
                newName: "IX_State_TimesSent_MTime");

            migrationBuilder.RenameIndex(
                name: "index_state_modificationtime",
                schema: "tsc",
                table: "IntegrationEventLog",
                newName: "IX_State_MTime");

            migrationBuilder.RenameIndex(
                name: "index_eventid_version",
                schema: "tsc",
                table: "IntegrationEventLog",
                newName: "IX_EventId_Version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Caculate",
                schema: "tsc",
                table: "PanelMetric",
                newName: "Value");

            migrationBuilder.RenameIndex(
                name: "IX_State_TimesSent_MTime",
                schema: "tsc",
                table: "IntegrationEventLog",
                newName: "index_state_timessent_modificationtime");

            migrationBuilder.RenameIndex(
                name: "IX_State_MTime",
                schema: "tsc",
                table: "IntegrationEventLog",
                newName: "index_state_modificationtime");

            migrationBuilder.RenameIndex(
                name: "IX_EventId_Version",
                schema: "tsc",
                table: "IntegrationEventLog",
                newName: "index_eventid_version");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "tsc",
                table: "PanelMetric",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
