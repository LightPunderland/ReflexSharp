using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReflexSharp_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSkins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EquippedSkin",
                table: "Users",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnedSkins",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquippedSkin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnedSkins",
                table: "Users");
        }
    }
}
