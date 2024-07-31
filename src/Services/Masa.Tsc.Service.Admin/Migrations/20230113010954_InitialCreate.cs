// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tsc");

            migrationBuilder.CreateTable(
                name: "Directory",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TimesSent = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Panel",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Width = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Left = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Top = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtensionData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PanelMetric",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PanelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caculate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelMetric", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Interval = table.Column<short>(type: "smallint", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TimeZone = table.Column<short>(type: "smallint", nullable: false),
                    TimeZoneOffset = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instrument",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Layer = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsRoot = table.Column<bool>(type: "bit", nullable: false),
                    Lable = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DirectoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGlobal = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instrument_Directory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalSchema: "tsc",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directory_Name",
                schema: "tsc",
                table: "Directory",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Directory_UserId",
                schema: "tsc",
                table: "Directory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrument_DirectoryId",
                schema: "tsc",
                table: "Instrument",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId_Version",
                schema: "tsc",
                table: "IntegrationEventLog",
                columns: new[] { "EventId", "RowVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_State_MTime",
                schema: "tsc",
                table: "IntegrationEventLog",
                columns: new[] { "State", "ModificationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_State_TimesSent_MTime",
                schema: "tsc",
                table: "IntegrationEventLog",
                columns: new[] { "State", "TimesSent", "ModificationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Setting_UserId",
                schema: "tsc",
                table: "Setting",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instrument",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "IntegrationEventLog",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Panel",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "PanelMetric",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Setting",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Directory",
                schema: "tsc");
        }
    }
}
