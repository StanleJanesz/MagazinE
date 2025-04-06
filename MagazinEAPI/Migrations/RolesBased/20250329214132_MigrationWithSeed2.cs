using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinEAPI.Migrations.RolesBased
{
    /// <inheritdoc />
    public partial class MigrationWithSeed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174e-3b0e-446f-86af-483d56fd7214",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGjtaYT1QQugEtDzxCHL5AD80LCJ9rX+EAYl44HijtqzIHZXfphoRhNjjBW2WujvIw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174e-3b0e-486f-86af-483d56fd7213",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENKzyCo5g+I320Z15/ZxFZopnss/ggALZ73kN4z2QWrVG+NRZz9bk7L+w7dSBYa4Ag==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174r-3b0e-446f-86af-483d56fd7211",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECAtlY2RA/vjLYvmweL9IBRZJl25Few+FUY8QMsC3/rJADAsQxzWxlD4QR4/ZJoSAw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174r-3b0e-446f-86af-483d56fd7214",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOAXkcsy7IiVg45vv09dUPqtwHOzbwnO1kOLG7lC4iVA9G8VOr2qjWxBcGtQN1zC+w==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w179e-3b0e-446f-86af-483d56fd7212",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDaQJNcmsFaqlvYpylJ8LL9S9aYOCTsB7PtdbsCQsymITYP5lz9PK77Q3FJw+xFmGw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174e-3b0e-446f-86af-483d56fd7214",
                column: "PasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174e-3b0e-486f-86af-483d56fd7213",
                column: "PasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174r-3b0e-446f-86af-483d56fd7211",
                column: "PasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w174r-3b0e-446f-86af-483d56fd7214",
                column: "PasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c5w179e-3b0e-446f-86af-483d56fd7212",
                column: "PasswordHash",
                value: null);
        }
    }
}
