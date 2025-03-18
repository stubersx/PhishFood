using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhishFood.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondaryIteration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "Attempts",
            table: "Result",
            nullable: true,
            defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Attempts",
            table: "Result");
        }
    }
}
