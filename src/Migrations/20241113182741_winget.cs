using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReflexSharp_BE.Migrations
{
    /// <inheritdoc />
    public partial class winget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rank",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AddColumn<int>(
                name: "Coins",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XP",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coins",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "XP",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Rank",
                table: "Users",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
