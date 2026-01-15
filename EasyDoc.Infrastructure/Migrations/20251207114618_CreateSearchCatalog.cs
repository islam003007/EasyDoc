using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateSearchCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                    IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'FTS_Doctors_Catalog')
                    CREATE FULLTEXT CATALOG FTS_Doctors_Catalog AS DEFAULT;", suppressTransaction: true); // Create The Catalog
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                    IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'FTS_Doctors_Catalog')
                    DROP FULLTEXT CATALOG FTS_Doctors_Catalog;", suppressTransaction: true); ; // Drop the Catalog
        }
    }
}
