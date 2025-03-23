using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinEAPI.Migrations
{
    /// <inheritdoc />
    public partial class RegisterRequestfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "RegisterRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccesfull",
                table: "RegisterRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDateTime",
                table: "RegisterRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccesfull",
                table: "RegisterRequests");

            migrationBuilder.DropColumn(
                name: "RegisterDateTime",
                table: "RegisterRequests");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "RegisterRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
