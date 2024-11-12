using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrivialAPI.Migrations
{
    /// <inheritdoc />
    public partial class UsertotalGamesParam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalGames",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalGames",
                table: "User");
        }
    }
}
