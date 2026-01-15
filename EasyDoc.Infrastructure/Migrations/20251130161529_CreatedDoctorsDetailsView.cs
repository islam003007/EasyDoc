using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatedDoctorsDetailsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW dbo.vw_DoctorDetails
                AS
                SELECT 
                    Doctors.Id AS Id,
                    Doctors.UserId as UserId,
                    Doctors.PersonName AS PersonName,
                    Doctors.PhoneNumber As PhoneNumber,
                    Doctors.ClinicAddress AS ClinicAddress,
                    Doctors.Description AS Description,
                    Doctors.ProfilePictureUrl AS ProfilePictureUrl,
                    Doctors.IdCardPictureUrl as IdCardPictureUrl,
                    Doctors.DefaultAppointmentTimeInMinutes As DefaultAppointmentTimeInMinutes,
                    Doctors.IsVisible As IsVisible,
                    Departments.Name AS Department,
                    Cities.Name AS City
                FROM dbo.Doctors
                JOIN dbo.Departments ON Doctors.DepartmentId = Departments.Id
                JOIN dbo.Cities ON Doctors.CityId = Cities.Id;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_DoctorDetails;");
        }
    }
}
