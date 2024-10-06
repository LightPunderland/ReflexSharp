using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReflexSharp_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddScoresTableWithMinorBugFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_scores_Users_id",
                table: "scores");

            migrationBuilder.CreateIndex(
                name: "IX_scores_user_id",
                table: "scores",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_scores_Users_user_id",
                table: "scores",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_scores_Users_user_id",
                table: "scores");

            migrationBuilder.DropIndex(
                name: "IX_scores_user_id",
                table: "scores");

            migrationBuilder.AddForeignKey(
                name: "FK_scores_Users_id",
                table: "scores",
                column: "id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
