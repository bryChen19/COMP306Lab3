using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _301379036_chen_lab3.Migrations
{
    /// <inheritdoc />
    public partial class addtopicandhost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Episodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Episodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Host",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Episodes");
        }
    }
}
