using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                    IF NOT EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('Doctors'))
                    BEGIN
                        CREATE FULLTEXT INDEX ON Doctors
                        (
                            NormalizedName LANGUAGE 0,   -- Non-linguistic
                            MetaphoneKeys LANGUAGE 0     -- Non-linguistic
                        )
                        KEY INDEX PK_Doctors           
                        ON FTS_Doctors_Catalog
                        WITH CHANGE_TRACKING AUTO;     -- Automatically keep index updated
                    END", suppressTransaction: true); // Create The Tndexes
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider != "Microsoft.EntityFrameworkCore.SqlServer")
                return; // Else: implement for other sql providers.

            migrationBuilder.Sql(@"
                    IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('Doctors'))
                    DROP FULLTEXT INDEX ON Doctors;", suppressTransaction: true); // Drop The Indexes
        }
    }
}
