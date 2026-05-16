using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationLevelToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "Documents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Documents");
        }
    }
}
