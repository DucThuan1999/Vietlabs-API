using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Thêm short_name, chuyển giá trị code cũ sang short_name,
    /// gán mã mới NTP-001, NTP-002, ...
    /// </summary>
    public partial class SubcontractorAbbreviationNtpCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "short_name",
                table: "subcontractor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE subcontractor
SET short_name = NULLIF(LTRIM(RTRIM(code)), N'');

;WITH ordered AS (
  SELECT subcontractor_id,
         ROW_NUMBER() OVER (ORDER BY COALESCE(created_at, CAST('1900-01-01' AS datetime2)), subcontractor_id) AS rn
  FROM subcontractor
)
UPDATE s
SET code = CONCAT(N'NTP-', FORMAT(o.rn, N'D3'))
FROM subcontractor s
INNER JOIN ordered o ON s.subcontractor_id = o.subcontractor_id;
");

            migrationBuilder.CreateIndex(
                name: "i_x_subcontractor_short_name",
                table: "subcontractor",
                column: "short_name",
                unique: true,
                filter: "[short_name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_subcontractor_short_name",
                table: "subcontractor");

            migrationBuilder.DropColumn(
                name: "short_name",
                table: "subcontractor");
        }
    }
}
