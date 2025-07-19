using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGameApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveCount",
                table: "Games");

            migrationBuilder.AlterColumn<int>(
                name: "Symbol",
                table: "Moves",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(char),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Player",
                table: "Moves",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPlayer",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(char),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player",
                table: "Moves");

            migrationBuilder.AlterColumn<char>(
                name: "Symbol",
                table: "Moves",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<char>(
                name: "CurrentPlayer",
                table: "Games",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "MoveCount",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
