using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotDb.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyIterationSaveDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IterationSaveDish",
                table: "UserActions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IterationSaveDish",
                table: "UserActions");
        }
    }
}
