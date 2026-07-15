/*
  Vietlabs: bảng cấu hình singleton tên hiển thị giấy phép đăng ký
  - Khớp EF entity VietLab.Models.RegistrationPermitLabelConfig + migration 20260525120000_AddRegistrationPermitLabelConfig
  - SQL Server
  - Idempotent: chỉ tạo bảng nếu chưa có; chỉ seed dòng mặc định nếu chưa tồn tại GUID seed
  - Field UI cố định: "GIẤY PHÉP ĐĂNG KÝ"
  - Giá trị seed display_name: "NĐ 22/2026"
*/

-- 1) Tạo bảng
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'registration_permit_label_config')
BEGIN
    CREATE TABLE registration_permit_label_config (
        registration_permit_label_config_id uniqueidentifier NOT NULL
            CONSTRAINT PK_registration_permit_label_config PRIMARY KEY,
        display_name nvarchar(200) NOT NULL,
        created_at datetime2 NOT NULL
            CONSTRAINT DF_registration_permit_label_config_created_at DEFAULT SYSUTCDATETIME(),
        updated_at datetime2 NULL,
        updated_by uniqueidentifier NULL,
        CONSTRAINT CK_registration_permit_label_config_display_name
            CHECK (LTRIM(RTRIM(display_name)) <> N'')
    );
END
GO

-- 2) Seed singleton
DECLARE @SeedId uniqueidentifier = '0891de10-6c6a-4b54-8373-3fd73ef4ac0c';

IF NOT EXISTS (
    SELECT 1 FROM registration_permit_label_config
    WHERE registration_permit_label_config_id = @SeedId
)
BEGIN
    INSERT INTO registration_permit_label_config (
        registration_permit_label_config_id,
        display_name,
        created_at,
        updated_at,
        updated_by
    )
    VALUES (
        @SeedId,
        N'NĐ 22/2026',
        SYSUTCDATETIME(),
        NULL,
        NULL
    );
END
GO

/*
  3) (Tuỳ chọn, môi trường dev) Gỡ bảng — CHỈ chạy khi chắc chắn không còn dữ liệu cần giữ

IF OBJECT_ID(N'dbo.registration_permit_label_config', N'U') IS NOT NULL
    DROP TABLE dbo.registration_permit_label_config;
GO
*/
