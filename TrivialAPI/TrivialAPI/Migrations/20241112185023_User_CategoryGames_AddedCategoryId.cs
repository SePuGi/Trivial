using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrivialAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserCategoryGamesAddedCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CategoryGames",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CategoryGames");
        }
    }
}
