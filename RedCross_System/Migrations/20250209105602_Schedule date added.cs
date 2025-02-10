using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedCross_System.Migrations
{
    /// <inheritdoc />
    public partial class Scheduledateadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "Donations",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                table: "Donations");
        }
    }
}
