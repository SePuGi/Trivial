using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrivialAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserCategoryGamesModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryGames_User_UserId",
                table: "CategoryGames");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CategoryGames",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryGames_User_UserId",
                table: "CategoryGames",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryGames_User_UserId",
                table: "CategoryGames");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CategoryGames",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryGames_User_UserId",
                table: "CategoryGames",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
