using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProgramlamaProje.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
        name: "Duration",
        table: "Courses",
        type: "TEXT",
        maxLength: 50,
        nullable: false,
        defaultValue: "Belirtilmedi");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Courses",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Orta Seviye");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Courses",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Türkçe");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Duration", table: "Courses");
            migrationBuilder.DropColumn(name: "Level", table: "Courses");
            migrationBuilder.DropColumn(name: "Language", table: "Courses");
            migrationBuilder.DropColumn(name: "Price", table: "Courses");
        }
    }
}
