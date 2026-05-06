using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdmissionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPrograms_AdmissionId_ProgramId",
                table: "AdmissionPrograms",
                columns: new[] { "AdmissionId", "ProgramId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdmissionPrograms_AdmissionId_ProgramId",
                table: "AdmissionPrograms");
        }
    }
}
