using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SearchSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                    CREATE OR ALTER PROCEDURE SearchDoctorsByRankAndPaginate
                    (
                        @LexicalSearchTerm NVARCHAR(4000),
                        @PhoneticSearchTerms NVARCHAR(4000),
                        @CityId UNIQUEIDENTIFIER = NULL,
                        @DepartmentId UNIQUEIDENTIFIER = NUll,
                        @PageNumber INT = 1,
                        @PageSize INT = 10
                    )
                    AS
                    BEGIN
                        SET NOCOUNT ON;

                        DECLARE @MaxPageSize INT = 100;
                        IF @PageNumber < 1 SET @PageNumber = 1;
                        IF @PageSize < 1 SET @PageSize = 1;
                        IF @PageSize > @MaxPageSize SET @PageSize = @MaxPageSize;

                        DECLARE @OffsetRows INT = (@PageNumber - 1) * @PageSize;

                        WITH LexicalResults AS (
                            SELECT KEY_TBL.[KEY] AS DoctorId, KEY_TBL.[RANK] AS NameRank
                            FROM CONTAINSTABLE(Doctors, NormalizedName, @LexicalSearchTerm, LANGUAGE 0) AS KEY_TBL
                        ),
                        PhoneticResults AS (
                            SELECT KEY_TBL.[KEY] AS DoctorId, KEY_TBL.[RANK] AS KeyRank
                            FROM CONTAINSTABLE(Doctors, MetaphoneKeys, @PhoneticSearchTerms, LANGUAGE 0) AS KEY_TBL
                        ),
                        FullResults AS ( SELECT
                                            ISNULL(l.DoctorId, p.DoctorId) AS DoctorId,
                                            ISNULL(l.NameRank, 0) AS NameRank,
                                            ISNULL(p.KeyRank, 0) AS KeyRank,
                                            ISNULL(l.NameRank, 0) + (ISNULL(p.KeyRank, 0) * 0.5) AS FinalRank
                                         FROM LexicalResults l
                                         FULL OUTER JOIN PhoneticResults p ON l.DoctorId = p.DoctorId
                        )
                        SELECT
                            Doctors.Id AS Id,
                            Doctors.PersonName AS PersonName,
                            Doctors.ProfilePictureURL AS ProfilePictureURL,
                            d.Name AS Department,
                            c.Name AS City,
                            f.NameRank as LexicalRank,
                            f.KeyRank as PhoneticRank
                        FROM FullResults f
                        JOIN Doctors ON Doctors.Id = f.DoctorId
                        JOIN Departments d ON Doctors.DepartmentId = d.Id
                        JOIN Cities c ON Doctors.CityId = c.Id
                        WHERE
                            Doctors.IsVisible = 1
                            AND (@CityId IS NULL OR Doctors.CityId = @CityId) 
                            AND (@DepartmentId IS NULL OR Doctors.DepartmentId = @DepartmentId)
                        ORDER BY f.FinalRank DESC, f.DoctorId ASC
                        OFFSET @OffsetRows ROWS FETCH NEXT @PageSize ROWS ONLY;
                    END"); // Creeate the SP
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                    DROP PROCEDURE IF EXISTS SearchDoctorsByRankAndPaginate;"); // Drop The SP
        }
    }
}
