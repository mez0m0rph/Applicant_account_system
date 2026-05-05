using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdmissionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantEmailAndManagerFlowFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Admissions_ApplicantUserId",
                table: "Admissions");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Admissions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantEmail",
                table: "Admissions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantEmail",
                table: "Admissions");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Admissions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_ApplicantUserId",
                table: "Admissions",
                column: "ApplicantUserId",
                unique: true);
        }
    }
}
