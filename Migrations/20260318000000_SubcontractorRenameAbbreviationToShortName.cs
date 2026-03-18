using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// DB đã chạy migration cũ (cột abbreviation): đổi tên cột → short_name.
    /// DB mới chỉ có short_name: không thay đổi.
    /// </summary>
    public partial class SubcontractorRenameAbbreviationToShortName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'dbo.subcontractor', N'abbreviation') IS NOT NULL
   AND COL_LENGTH(N'dbo.subcontractor', N'short_name') IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_subcontractor_abbreviation' AND object_id = OBJECT_ID(N'dbo.subcontractor'))
        EXEC(N'DROP INDEX i_x_subcontractor_abbreviation ON dbo.subcontractor');
    EXEC sp_rename N'dbo.subcontractor.abbreviation', N'short_name', N'COLUMN';
END
");
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'dbo.subcontractor', N'short_name') IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_subcontractor_short_name' AND object_id = OBJECT_ID(N'dbo.subcontractor'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX i_x_subcontractor_short_name ON dbo.subcontractor(short_name) WHERE [short_name] IS NOT NULL;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'dbo.subcontractor', N'short_name') IS NOT NULL
   AND COL_LENGTH(N'dbo.subcontractor', N'abbreviation') IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_subcontractor_short_name' AND object_id = OBJECT_ID(N'dbo.subcontractor'))
        EXEC(N'DROP INDEX i_x_subcontractor_short_name ON dbo.subcontractor');
    EXEC sp_rename N'dbo.subcontractor.short_name', N'abbreviation', N'COLUMN';
    CREATE UNIQUE NONCLUSTERED INDEX i_x_subcontractor_abbreviation ON dbo.subcontractor(abbreviation) WHERE [abbreviation] IS NOT NULL;
END
");
        }
    }
}
