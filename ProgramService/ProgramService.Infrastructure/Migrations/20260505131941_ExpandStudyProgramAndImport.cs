using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgramService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandStudyProgramAndImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StudyPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EducationForm",
                table: "StudyPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "StudyPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "StudyPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPrograms_ExternalId",
                table: "StudyPrograms",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudyPrograms_ExternalId",
                table: "StudyPrograms");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StudyPrograms");

            migrationBuilder.DropColumn(
                name: "EducationForm",
                table: "StudyPrograms");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "StudyPrograms");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "StudyPrograms");
        }
    }
}
