using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mushroomAPI.Migrations
{
    /// <inheritdoc />
    public partial class userlocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coordinates_Mushrooms_MushroomId",
                table: "Coordinates");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Coordinates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Coordinates",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Coordinates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "UserId", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.UpdateData(
                table: "Coordinates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "UserId", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.UpdateData(
                table: "Coordinates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "UserId", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.UpdateData(
                table: "Coordinates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "UserId", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.UpdateData(
                table: "Coordinates",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "UserId", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.UpdateData(
                table: "Coordinates",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "UserId", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_Coordinates_Mushrooms_MushroomId",
                table: "Coordinates",
                column: "MushroomId",
                principalTable: "Mushrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coordinates_Mushrooms_MushroomId",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Coordinates");

            migrationBuilder.AddForeignKey(
                name: "FK_Coordinates_Mushrooms_MushroomId",
                table: "Coordinates",
                column: "MushroomId",
                principalTable: "Mushrooms",
                principalColumn: "Id");
        }
    }
}
