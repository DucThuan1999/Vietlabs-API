using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>Bảng lưu callback inbound từ AMIS/MISA ACT Open.</summary>
    public partial class AddAmisCallbackLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'amis_callback_log')
BEGIN
    CREATE TABLE amis_callback_log (
        amis_callback_log_id uniqueidentifier NOT NULL CONSTRAINT PK_amis_callback_log PRIMARY KEY,
        success bit NOT NULL,
        error_code nvarchar(100) NULL,
        error_message nvarchar(2000) NULL,
        signature nvarchar(256) NULL,
        data_type int NOT NULL,
        data nvarchar(max) NULL,
        org_company_code nvarchar(200) NULL,
        app_id nvarchar(100) NULL,
        is_signature_valid bit NOT NULL,
        received_at datetime2 NOT NULL,
        processed_at datetime2 NULL,
        processing_error nvarchar(2000) NULL
    );

    CREATE INDEX IX_amis_callback_log_received_at ON amis_callback_log (received_at);
    CREATE INDEX IX_amis_callback_log_data_type ON amis_callback_log (data_type);
    CREATE INDEX IX_amis_callback_log_is_signature_valid ON amis_callback_log (is_signature_valid);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS amis_callback_log;");
        }
    }
}
