using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrivialAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserCategoryGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TotalGames",
                table: "User");

            migrationBuilder.CreateTable(
                name: "CategoryGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    TotalGames = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryGames_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGames_UserId",
                table: "CategoryGames",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryGames");

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswers",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalGames",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
