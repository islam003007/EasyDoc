using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatedPublicQueryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Doctors_CityId_DepartmentId",
                table: "Doctors",
                columns: new[] { "CityId", "DepartmentId" },
                filter: "[IsVisible] = 1")
                .Annotation("SqlServer:Include", new[] { "Id", "PersonName", "IdCardPictureUrl" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctors_CityId_DepartmentId",
                table: "Doctors");
        }
    }
}
