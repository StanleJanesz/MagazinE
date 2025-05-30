using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinEAPI.Migrations.RolesBased
{
    /// <inheritdoc />
    public partial class IntegrationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Bans");

            migrationBuilder.AlterColumn<int>(
                name: "SolvedById",
                table: "UnbanRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "CommentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "CommentReports");

            migrationBuilder.AlterColumn<int>(
                name: "SolvedById",
                table: "UnbanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Result",
                table: "CommentReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Bans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
