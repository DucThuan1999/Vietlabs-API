/*
  Chỉ thêm cột DisplayName (không migrate dữ liệu).
  Dùng khi đã chạy dotnet ef database update hoặc chỉ cần bổ sung cột thủ công.
*/

SET NOCOUNT ON;

IF COL_LENGTH('dbo.analysis_item', 'display_name_vi') IS NULL
    ALTER TABLE dbo.analysis_item ADD display_name_vi NVARCHAR(MAX) NULL;

IF COL_LENGTH('dbo.analysis_item', 'display_name_en') IS NULL
    ALTER TABLE dbo.analysis_item ADD display_name_en NVARCHAR(MAX) NULL;

IF COL_LENGTH('dbo.quotation_item', 'item_display_name_vi') IS NULL
    ALTER TABLE dbo.quotation_item ADD item_display_name_vi NVARCHAR(MAX) NULL;

IF COL_LENGTH('dbo.quotation_item', 'item_display_name_en') IS NULL
    ALTER TABLE dbo.quotation_item ADD item_display_name_en NVARCHAR(MAX) NULL;

-- Tùy chọn: nới name_vi/name_en nếu vẫn NVARCHAR(500)
IF EXISTS (
    SELECT 1 FROM sys.columns c
    JOIN sys.tables t ON t.object_id = c.object_id
    WHERE t.name = N'analysis_item' AND c.name = N'name_vi'
      AND c.max_length > 0 AND c.max_length <> -1
)
    ALTER TABLE dbo.analysis_item ALTER COLUMN name_vi NVARCHAR(MAX) NULL;

IF EXISTS (
    SELECT 1 FROM sys.columns c
    JOIN sys.tables t ON t.object_id = c.object_id
    WHERE t.name = N'analysis_item' AND c.name = N'name_en'
      AND c.max_length > 0 AND c.max_length <> -1
)
    ALTER TABLE dbo.analysis_item ALTER COLUMN name_en NVARCHAR(MAX) NULL;

PRINT N'Hoàn tất: đã đảm bảo các cột display name tồn tại.';
