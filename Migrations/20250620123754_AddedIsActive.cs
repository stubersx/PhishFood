using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhishFood.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Trainings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Testings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Testings");
        }
    }
}
