using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_IdentityId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_IdentityId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "IdentityId",
                table: "Patients",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_IdentityId",
                table: "Patients",
                newName: "IX_Patients_UserId");

            migrationBuilder.RenameColumn(
                name: "IdentityId",
                table: "Doctors",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_IdentityId",
                table: "Doctors",
                newName: "IX_Doctors_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Patients",
                newName: "IdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                newName: "IX_Patients_IdentityId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Doctors",
                newName: "IdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                newName: "IX_Doctors_IdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_IdentityId",
                table: "Doctors",
                column: "IdentityId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_IdentityId",
                table: "Patients",
                column: "IdentityId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
