using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserEmailView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW dbo.vw_user_read AS
                SELECT
                    Id    AS UserId,
                    Email
                FROM [identity].Users;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_user_read;");
        }
    }
}
