using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotDb.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyDishUserAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DishUserAccess",
                table: "DishUserAccess");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "DishUserAccess",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishUserAccess",
                table: "DishUserAccess",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DishUserAccess_DishId",
                table: "DishUserAccess",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DishUserAccess",
                table: "DishUserAccess");

            migrationBuilder.DropIndex(
                name: "IX_DishUserAccess_DishId",
                table: "DishUserAccess");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DishUserAccess");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishUserAccess",
                table: "DishUserAccess",
                column: "DishId");
        }
    }
}
