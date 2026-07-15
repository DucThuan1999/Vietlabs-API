using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Bảng lịch \"thông tin ban hành\" PDF báo giá + seed mặc định.
    /// </summary>
    public partial class AddQuotationIssueInfo : Migration
    {
        private const string SeedQuotationIssueInfoId = "bb5703ee-219a-4a2e-a81f-3674bf00614b";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'quotation_issue_info')
BEGIN
    CREATE TABLE quotation_issue_info (
        quotation_issue_info_id uniqueidentifier NOT NULL CONSTRAINT PK_quotation_issue_info PRIMARY KEY,
        content nvarchar(max) NOT NULL,
        start_date datetime2 NOT NULL,
        end_date datetime2 NULL,
        description nvarchar(max) NULL,
        status nvarchar(max) NOT NULL,
        created_at datetime2 NOT NULL,
        updated_at datetime2 NULL,
        created_by uniqueidentifier NULL,
        updated_by uniqueidentifier NULL
    );
END
");

            migrationBuilder.Sql($@"
IF NOT EXISTS (SELECT 1 FROM quotation_issue_info WHERE quotation_issue_info_id = '{SeedQuotationIssueInfoId}')
BEGIN
    INSERT INTO quotation_issue_info (quotation_issue_info_id, content, start_date, end_date, description, status, created_at, updated_at, created_by, updated_by)
    VALUES (
        '{SeedQuotationIssueInfoId}',
        N'VLAB01.KD   Lần BH: 02    Ngày BH: 05/05/2022',
        '2000-01-01T00:00:00',
        NULL,
        N'Mặc định',
        N'Active',
        SYSUTCDATETIME(),
        NULL,
        NULL,
        NULL
    );
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS quotation_issue_info;");
        }
    }
}
