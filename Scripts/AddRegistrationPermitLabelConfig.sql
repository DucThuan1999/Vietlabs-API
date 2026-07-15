/*
  Vietlabs: cấu hình singleton tên hiển thị thay NĐ107
  - Bảng: registration_permit_label_config
  - Field cố định (UI): "GIẤY PHÉP ĐĂNG KÝ"
  - Giá trị seed mặc định (display_name): "NĐ 22/2026"
  - Khớp EF entity RegistrationPermitLabelConfig
  - Idempotent — chạy nhiều lần an toàn
*/

SET NOCOUNT ON;

DECLARE @SeedId UNIQUEIDENTIFIER = '0891de10-6c6a-4b54-8373-3fd73ef4ac0c';
DECLARE @LegacySeedId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @DefaultDisplayName NVARCHAR(200) = N'NĐ 22/2026';

-- 1) Tạo bảng
IF OBJECT_ID(N'dbo.registration_permit_label_config', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.registration_permit_label_config (
        registration_permit_label_config_id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_registration_permit_label_config PRIMARY KEY,
        display_name NVARCHAR(200) NOT NULL,
        created_at DATETIME2 NOT NULL
            CONSTRAINT DF_registration_permit_label_config_created_at DEFAULT SYSUTCDATETIME(),
        updated_at DATETIME2 NULL,
        updated_by UNIQUEIDENTIFIER NULL,
        CONSTRAINT CK_registration_permit_label_config_display_name
            CHECK (LTRIM(RTRIM(display_name)) <> N'')
    );
    PRINT N'Đã tạo bảng dbo.registration_permit_label_config';
END
ELSE
    PRINT N'Bỏ qua: dbo.registration_permit_label_config đã tồn tại';

-- 2) Migrate placeholder seed id (nếu đã chạy bản cũ)
IF EXISTS (
    SELECT 1 FROM dbo.registration_permit_label_config
    WHERE registration_permit_label_config_id = @LegacySeedId
)
AND NOT EXISTS (
    SELECT 1 FROM dbo.registration_permit_label_config
    WHERE registration_permit_label_config_id = @SeedId
)
BEGIN
    UPDATE dbo.registration_permit_label_config
    SET registration_permit_label_config_id = @SeedId
    WHERE registration_permit_label_config_id = @LegacySeedId;
    PRINT N'Đã migrate seed id placeholder -> id thật';
END

-- 3) Seed singleton (chỉ INSERT nếu chưa có GUID seed)
IF NOT EXISTS (
    SELECT 1
    FROM dbo.registration_permit_label_config
    WHERE registration_permit_label_config_id = @SeedId
)
BEGIN
    INSERT INTO dbo.registration_permit_label_config (
        registration_permit_label_config_id,
        display_name,
        created_at,
        updated_at,
        updated_by
    )
    VALUES (
        @SeedId,
        @DefaultDisplayName,
        SYSUTCDATETIME(),
        NULL,
        NULL
    );
    PRINT N'Đã seed registration_permit_label_config: display_name = NĐ 22/2026';
END
ELSE
    PRINT N'Bỏ qua seed: bản ghi singleton đã tồn tại';

-- 4) Cập nhật display_name về mặc định nếu bản ghi seed còn rỗng/null
UPDATE dbo.registration_permit_label_config
SET
    display_name = @DefaultDisplayName,
    updated_at = SYSUTCDATETIME()
WHERE registration_permit_label_config_id = @SeedId
  AND (display_name IS NULL OR LTRIM(RTRIM(display_name)) = N'');

GO
