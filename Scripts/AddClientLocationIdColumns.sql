/*
  Vietlabs: thêm cột location ID cho bảng client
  - country_id, province_id, ward_id (nullable uniqueidentifier)
  - Index + FK tới country / province / ward
  - Idempotent — chạy nhiều lần an toàn
*/

SET NOCOUNT ON;

-- 1) Thêm cột
IF COL_LENGTH('dbo.client', 'country_id') IS NULL
BEGIN
    ALTER TABLE dbo.client ADD country_id UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm dbo.client.country_id';
END
ELSE
    PRINT N'Bỏ qua: dbo.client.country_id đã tồn tại';

IF COL_LENGTH('dbo.client', 'province_id') IS NULL
BEGIN
    ALTER TABLE dbo.client ADD province_id UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm dbo.client.province_id';
END
ELSE
    PRINT N'Bỏ qua: dbo.client.province_id đã tồn tại';

IF COL_LENGTH('dbo.client', 'ward_id') IS NULL
BEGIN
    ALTER TABLE dbo.client ADD ward_id UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm dbo.client.ward_id';
END
ELSE
    PRINT N'Bỏ qua: dbo.client.ward_id đã tồn tại';

-- 1b) Đảm bảo ward lưu Unicode (nvarchar)
IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = N'dbo'
      AND TABLE_NAME = N'client'
      AND COLUMN_NAME = N'ward'
      AND DATA_TYPE = N'varchar'
)
BEGIN
    ALTER TABLE dbo.client ALTER COLUMN ward NVARCHAR(MAX) NULL;
    PRINT N'Đã đổi dbo.client.ward từ varchar sang nvarchar';
END
ELSE IF COL_LENGTH('dbo.client', 'ward') IS NOT NULL
    PRINT N'Bỏ qua: dbo.client.ward đã là nvarchar';

-- 2) Index
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_client_country_id'
      AND object_id = OBJECT_ID(N'dbo.client')
)
BEGIN
    CREATE INDEX IX_client_country_id ON dbo.client (country_id);
    PRINT N'Đã tạo index IX_client_country_id';
END
ELSE
    PRINT N'Bỏ qua: IX_client_country_id đã tồn tại';

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_client_province_id'
      AND object_id = OBJECT_ID(N'dbo.client')
)
BEGIN
    CREATE INDEX IX_client_province_id ON dbo.client (province_id);
    PRINT N'Đã tạo index IX_client_province_id';
END
ELSE
    PRINT N'Bỏ qua: IX_client_province_id đã tồn tại';

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_client_ward_id'
      AND object_id = OBJECT_ID(N'dbo.client')
)
BEGIN
    CREATE INDEX IX_client_ward_id ON dbo.client (ward_id);
    PRINT N'Đã tạo index IX_client_ward_id';
END
ELSE
    PRINT N'Bỏ qua: IX_client_ward_id đã tồn tại';

-- 3) Foreign keys
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_client_country_country_id'
      AND parent_object_id = OBJECT_ID(N'dbo.client')
)
BEGIN
    ALTER TABLE dbo.client
    ADD CONSTRAINT FK_client_country_country_id
        FOREIGN KEY (country_id) REFERENCES dbo.country (country_id)
        ON DELETE NO ACTION;
    PRINT N'Đã tạo FK FK_client_country_country_id';
END
ELSE
    PRINT N'Bỏ qua: FK_client_country_country_id đã tồn tại';

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_client_province_province_id'
      AND parent_object_id = OBJECT_ID(N'dbo.client')
)
BEGIN
    ALTER TABLE dbo.client
    ADD CONSTRAINT FK_client_province_province_id
        FOREIGN KEY (province_id) REFERENCES dbo.province (province_id)
        ON DELETE NO ACTION;
    PRINT N'Đã tạo FK FK_client_province_province_id';
END
ELSE
    PRINT N'Bỏ qua: FK_client_province_province_id đã tồn tại';

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_client_ward_ward_id'
      AND parent_object_id = OBJECT_ID(N'dbo.client')
)
BEGIN
    ALTER TABLE dbo.client
    ADD CONSTRAINT FK_client_ward_ward_id
        FOREIGN KEY (ward_id) REFERENCES dbo.ward (ward_id)
        ON DELETE NO ACTION;
    PRINT N'Đã tạo FK FK_client_ward_ward_id';
END
ELSE
    PRINT N'Bỏ qua: FK_client_ward_ward_id đã tồn tại';

GO
