-- Thêm cột display_short_name (JSON Tiptap) cho analysis_item.
-- Idempotent — có thể chạy lại an toàn.

IF COL_LENGTH('dbo.analysis_item', 'display_short_name') IS NULL
BEGIN
    ALTER TABLE dbo.analysis_item
    ADD display_short_name NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm analysis_item.display_short_name';
END
ELSE
BEGIN
    PRINT N'Cột analysis_item.display_short_name đã tồn tại — bỏ qua.';
END;
