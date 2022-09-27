using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class tsc09261 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instrument",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Layer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsRoot = table.Column<bool>(type: "bit", nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "Panel",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UiType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PanelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Panel_Instrument_InstrumentId",
                        column: x => x.InstrumentId,
                        principalSchema: "tsc",
                        principalTable: "Instrument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Panel_Panel_PanelId",
                        column: x => x.PanelId,
                        principalSchema: "tsc",
                        principalTable: "Panel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PanelMetric",
                schema: "tsc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PanelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelMetric", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PanelMetric_Panel_PanelId",
                        column: x => x.PanelId,
                        principalSchema: "tsc",
                        principalTable: "Panel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PanelMetric_PanelId",
                schema: "tsc",
                table: "PanelMetric",
                column: "PanelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PanelMetric",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Panel",
                schema: "tsc");

            migrationBuilder.DropTable(
                name: "Instrument",
                schema: "tsc");
        }
    }
}
