using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Cho phép chỉ tiêu không gắn nhóm; xóa nhóm có name_vi = N/A (sau trim);
    /// gỡ FK trên chỉ tiêu, gói-nhóm, dòng báo giá; xóa quotation_analysis_group nếu có bảng.
    /// </summary>
    public partial class RemoveNaAnalysisGroupAndNullableAnalysisItemGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "analysis_group_id",
                table: "analysis_item",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: false);

            var sql = @"
DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);
INSERT INTO @ids (id)
SELECT analysis_group_id
FROM analysis_group
WHERE LTRIM(RTRIM(ISNULL(name_vi, N''))) = N'N/A';

UPDATE ai
SET analysis_group_id = NULL,
    updated_at = SYSUTCDATETIME()
FROM analysis_item ai
INNER JOIN @ids i ON ai.analysis_group_id = i.id;

DELETE pag
FROM package_analysis_group pag
INNER JOIN @ids i ON pag.analysis_group_id = i.id;

UPDATE qi
SET analysis_group_id = NULL,
    updated_at = SYSUTCDATETIME()
FROM quotation_item qi
INNER JOIN @ids i ON qi.analysis_group_id = i.id;

IF OBJECT_ID(N'quotation_analysis_group', N'U') IS NOT NULL
BEGIN
    DELETE qag
    FROM quotation_analysis_group qag
    INNER JOIN @ids i ON qag.analysis_group_id = i.id;
END

DELETE ag
FROM analysis_group ag
INNER JOIN @ids i ON ag.analysis_group_id = i.id;
";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "analysis_group_id",
                table: "analysis_item",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
