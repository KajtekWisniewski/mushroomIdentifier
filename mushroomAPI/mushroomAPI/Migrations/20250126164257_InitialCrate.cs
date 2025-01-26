using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace mushroomAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mushrooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ScientificName = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsEdible = table.Column<bool>(type: "boolean", nullable: false),
                    Habitat = table.Column<string>(type: "text", nullable: false),
                    Season = table.Column<string>(type: "text", nullable: false),
                    CommonNames = table.Column<string>(type: "text", nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mushrooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    SavedRecognitions = table.Column<string>(type: "text", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coordinates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    MushroomId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coordinates_Mushrooms_MushroomId",
                        column: x => x.MushroomId,
                        principalTable: "Mushrooms",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Mushrooms",
                columns: new[] { "Id", "Category", "CommonNames", "Description", "Habitat", "ImageUrls", "IsEdible", "LastUpdated", "Name", "ScientificName", "Season" },
                values: new object[,]
                {
                    { 1, 0, "[\"Meadow Mushroom\",\"Pink Bottom\"]", "Common edible mushroom found in grasslands", "Meadows and fields", "[\"https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Agaricus_campestris.jpg/1200px-Agaricus_campestris.jpg\",\"https://upload.wikimedia.org/wikipedia/commons/d/d7/Agaricus-campestris-michoacan.jpg\"]", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Field Mushroom", "Agaricus campestris", "Summer-Fall" },
                    { 2, 1, "[\"Death Cup\",\"Green Death Cap\"]", "One of the most poisonous mushrooms known", "Woodland", "[\"https://upload.wikimedia.org/wikipedia/commons/thumb/9/99/Amanita_phalloides_1.JPG/800px-Amanita_phalloides_1.JPG\",\"https://foodsafety.osu.edu/sites/cfi/files/imgclean/711-body-1692292442-1.jpg\"]", false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Death Cap", "Amanita phalloides", "Summer-Fall" },
                    { 3, 2, "[\"Porcini\",\"King Bolete\"]", "Prized edible mushroom with thick stem", "Mixed woodland", "[\"https://upload.wikimedia.org/wikipedia/commons/3/34/Boletus_edulis_IT.jpg\",\"https://i.ytimg.com/vi/m1yqWtcPFSQ/maxresdefault.jpg\"]", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Penny Bun", "Boletus edulis", "Late Summer-Fall" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsAdmin", "PasswordHash", "PasswordSalt", "SavedRecognitions", "Username" },
                values: new object[,]
                {
                    { 1, "admin@example.com", true, new byte[] { 232, 246, 141, 67, 232, 192, 159, 207, 134, 42, 63, 38, 46, 147, 75, 245, 79, 10, 139, 198, 40, 25, 118, 16, 36, 154, 138, 76, 117, 99, 135, 79, 42, 192, 147, 194, 211, 38, 11, 55, 62, 111, 229, 2, 166, 78, 122, 216, 64, 239, 74, 188, 228, 249, 59, 14, 151, 216, 52, 13, 246, 51, 79, 161 }, new byte[] { 175, 128, 96, 24, 152, 157, 168, 241, 14, 65, 66, 149, 210, 172, 223, 214, 63, 85, 16, 117, 97, 145, 136, 90, 143, 196, 70, 59, 100, 243, 63, 250, 113, 188, 169, 184, 230, 125, 44, 82, 97, 40, 188, 239, 4, 8, 115, 234, 77, 34, 73, 105, 155, 31, 121, 71, 3, 221, 238, 81, 104, 149, 177, 250, 215, 31, 202, 15, 101, 45, 107, 196, 217, 3, 250, 51, 135, 242, 127, 63, 89, 83, 101, 19, 131, 161, 137, 182, 163, 42, 242, 151, 130, 172, 227, 23, 130, 148, 183, 50, 61, 10, 208, 84, 200, 191, 119, 240, 202, 230, 6, 137, 120, 7, 251, 75, 57, 13, 245, 240, 0, 170, 140, 129, 166, 209, 47, 84 }, "[]", "admin" },
                    { 2, "user@example.com", false, new byte[] { 232, 246, 141, 67, 232, 192, 159, 207, 134, 42, 63, 38, 46, 147, 75, 245, 79, 10, 139, 198, 40, 25, 118, 16, 36, 154, 138, 76, 117, 99, 135, 79, 42, 192, 147, 194, 211, 38, 11, 55, 62, 111, 229, 2, 166, 78, 122, 216, 64, 239, 74, 188, 228, 249, 59, 14, 151, 216, 52, 13, 246, 51, 79, 161 }, new byte[] { 175, 128, 96, 24, 152, 157, 168, 241, 14, 65, 66, 149, 210, 172, 223, 214, 63, 85, 16, 117, 97, 145, 136, 90, 143, 196, 70, 59, 100, 243, 63, 250, 113, 188, 169, 184, 230, 125, 44, 82, 97, 40, 188, 239, 4, 8, 115, 234, 77, 34, 73, 105, 155, 31, 121, 71, 3, 221, 238, 81, 104, 149, 177, 250, 215, 31, 202, 15, 101, 45, 107, 196, 217, 3, 250, 51, 135, 242, 127, 63, 89, 83, 101, 19, 131, 161, 137, 182, 163, 42, 242, 151, 130, 172, 227, 23, 130, 148, 183, 50, 61, 10, 208, 84, 200, 191, 119, 240, 202, 230, 6, 137, 120, 7, 251, 75, 57, 13, 245, 240, 0, 170, 140, 129, 166, 209, 47, 84 }, "[]", "user" }
                });

            migrationBuilder.InsertData(
                table: "Coordinates",
                columns: new[] { "Id", "Latitude", "Longitude", "MushroomId" },
                values: new object[,]
                {
                    { 1, 51.507399999999997, -0.1278, 3 },
                    { 2, 48.8566, 2.3521999999999998, 3 },
                    { 3, 52.520000000000003, 13.404999999999999, 2 },
                    { 4, 45.421500000000002, -75.697199999999995, 2 },
                    { 5, 41.902799999999999, 12.4964, 1 },
                    { 6, 59.913899999999998, 10.7522, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_MushroomId",
                table: "Coordinates",
                column: "MushroomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinates");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Mushrooms");
        }
    }
}
