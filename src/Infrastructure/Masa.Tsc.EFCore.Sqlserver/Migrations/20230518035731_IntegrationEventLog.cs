// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    public partial class IntegrationEventLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpandContent",
                schema: "tsc",
                table: "IntegrationEventLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpandContent",
                schema: "tsc",
                table: "IntegrationEventLog");
        }
    }
}
