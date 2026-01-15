using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRemoved",
                schema: "identity",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "identity",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "Users",
                newName: "IsRemoved");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "identity",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
