using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations;

/// <summary>
/// Cột organization đã có trong model/snapshot nhưng chưa tồn tại trên DB thực tế — bổ sung khi thiếu.
/// </summary>
public partial class AddAnalysisItemOrganization : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.analysis_item', 'organization') IS NULL
BEGIN
    ALTER TABLE [dbo].[analysis_item]
        ADD [organization] NVARCHAR(500) NULL;
END
");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.analysis_item', 'organization') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [organization];
END
");
    }
}
