using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinEAPI.Migrations
{
    /// <inheritdoc />
    public partial class NameReaderSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174r-3b0e-446f-86af-483d56fd7211",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Grzegorz", "Brzęczyszczykiewicz" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174r-3b0e-446f-86af-483d56fd7211",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { null, null });
        }
    }
}
