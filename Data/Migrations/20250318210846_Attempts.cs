using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhishFood.Data.Migrations
{
    /// <inheritdoc />
    public partial class Attempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "Attempts",
            table: "Results",
            nullable: true,
            defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Attempts",
            table: "Results");
        }
    }
}
