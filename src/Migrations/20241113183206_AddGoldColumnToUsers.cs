using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReflexSharp_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddGoldColumnToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coins",
                table: "Users",
                newName: "Gold");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gold",
                table: "Users",
                newName: "Coins");
        }
    }
}
