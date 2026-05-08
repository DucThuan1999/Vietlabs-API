using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// DB hiện trường đã có schema; migration cũ scaffold nhầm toàn bộ CreateTable.
    /// Chỉ bổ sung cột đơn giá khi chưa có.
    /// </summary>
    public partial class AddUnitPriceToAnalysisItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.analysis_item', 'unit_price') IS NULL
BEGIN
    ALTER TABLE [dbo].[analysis_item]
        ADD [unit_price] DECIMAL(18,2) NOT NULL CONSTRAINT [df_analysis_item_unit_price] DEFAULT (0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.analysis_item', 'unit_price') IS NOT NULL
BEGIN
    DECLARE @dc SYSNAME;
    SELECT @dc = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c
        ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.analysis_item')
      AND c.name = N'unit_price';

    IF @dc IS NOT NULL
        EXEC(N'ALTER TABLE [dbo].[analysis_item] DROP CONSTRAINT [' + REPLACE(@dc, N']', N']]') + N']');

    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [unit_price];
END
");
        }
    }
}
