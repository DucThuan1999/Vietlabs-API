using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Bảng lịch VAT + seed 8% từ 2000-01-01 (EndDate NULL).
    /// </summary>
    public partial class AddVatRate : Migration
    {
        private const string SeedVatRateId = "11111111-1111-1111-1111-111111111111";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'vat_rate')
BEGIN
    CREATE TABLE vat_rate (
        vat_rate_id uniqueidentifier NOT NULL CONSTRAINT PK_vat_rate PRIMARY KEY,
        [percent] decimal(5,2) NOT NULL,
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
IF NOT EXISTS (SELECT 1 FROM vat_rate WHERE vat_rate_id = '{SeedVatRateId}')
BEGIN
    INSERT INTO vat_rate (vat_rate_id, [percent], start_date, end_date, description, status, created_at, updated_at, created_by, updated_by)
    VALUES ('{SeedVatRateId}', 8.00, '2000-01-01T00:00:00', NULL, N'Mặc định lịch sử 8%', N'Active', SYSUTCDATETIME(), NULL, NULL, NULL);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS vat_rate;");
        }
    }
}
