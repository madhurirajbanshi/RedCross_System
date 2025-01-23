using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedCross_System.Migrations
{
    /// <inheritdoc />
    public partial class Bloodrequirementtableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BloodRequirementId",
                table: "BloodIssues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BloodIssues_BloodRequirementId",
                table: "BloodIssues",
                column: "BloodRequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodIssues_BloodRequirements_BloodRequirementId",
                table: "BloodIssues",
                column: "BloodRequirementId",
                principalTable: "BloodRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodIssues_BloodRequirements_BloodRequirementId",
                table: "BloodIssues");

            migrationBuilder.DropIndex(
                name: "IX_BloodIssues_BloodRequirementId",
                table: "BloodIssues");

            migrationBuilder.DropColumn(
                name: "BloodRequirementId",
                table: "BloodIssues");
        }
    }
}
