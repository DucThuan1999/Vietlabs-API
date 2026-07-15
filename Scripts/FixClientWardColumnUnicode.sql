/*
  Sửa cột dbo.client.ward: varchar -> nvarchar để lưu tiếng Việt đúng.
  Idempotent — chạy nhiều lần an toàn.

  Sau khi chạy script này, chạy lại:
    python3 update_client_location_ids_from_xlsx.py --xlsx ... --apply
  để ghi lại giá trị ward từ Excel.
*/

SET NOCOUNT ON;

IF COL_LENGTH('dbo.client', 'ward') IS NULL
BEGIN
    RAISERROR(N'Cột dbo.client.ward không tồn tại.', 16, 1);
    RETURN;
END

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
ELSE
    PRINT N'Bỏ qua: dbo.client.ward đã là nvarchar hoặc kiểu khác';

GO
