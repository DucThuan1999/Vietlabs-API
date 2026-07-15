using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Cấu hình singleton tên hiển thị giấy phép đăng ký (thay NĐ107 trên UI).
    /// </summary>
    public partial class AddRegistrationPermitLabelConfig : Migration
    {
        private const string SeedId = "0891de10-6c6a-4b54-8373-3fd73ef4ac0c";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'registration_permit_label_config')
BEGIN
    CREATE TABLE registration_permit_label_config (
        registration_permit_label_config_id uniqueidentifier NOT NULL
            CONSTRAINT PK_registration_permit_label_config PRIMARY KEY,
        display_name nvarchar(200) NOT NULL,
        created_at datetime2 NOT NULL CONSTRAINT DF_registration_permit_label_config_created_at DEFAULT SYSUTCDATETIME(),
        updated_at datetime2 NULL,
        updated_by uniqueidentifier NULL,
        CONSTRAINT CK_registration_permit_label_config_display_name
            CHECK (LTRIM(RTRIM(display_name)) <> N'')
    );
END
");

            migrationBuilder.Sql($@"
IF NOT EXISTS (
    SELECT 1 FROM registration_permit_label_config
    WHERE registration_permit_label_config_id = '{SeedId}'
)
BEGIN
    INSERT INTO registration_permit_label_config (
        registration_permit_label_config_id, display_name, created_at, updated_at, updated_by
    )
    VALUES (
        '{SeedId}',
        N'NĐ 22/2026',
        SYSUTCDATETIME(),
        NULL,
        NULL
    );
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS registration_permit_label_config;");
        }
    }
}
