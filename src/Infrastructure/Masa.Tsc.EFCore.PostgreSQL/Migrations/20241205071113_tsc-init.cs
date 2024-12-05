using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.EFCore.PostgreSQL.Migrations
{
    public partial class tscinit : Migration
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExceptError",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Environment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Project = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Service = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptError", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Panel",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Width = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Left = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Top = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtensionData = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PanelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Caculate = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelMetric", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instrument",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Layer = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Model = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Sort = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsRoot = table.Column<bool>(type: "boolean", nullable: false),
                    Lable = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    DirectoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    EnableEdit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "IX_ExceptError_Environment_Project_Service_Type",
                schema: "tsc",
                table: "ExceptError",
                columns: new[] { "Environment", "Project", "Service", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Instrument_DirectoryId",
                schema: "tsc",
                table: "Instrument",
                column: "DirectoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptError",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Instrument",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Panel",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "PanelMetric",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Directory",
                schema: "tsc");
        }
    }
}
